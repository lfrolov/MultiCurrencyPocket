using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MultiCurrencyPocket.Services.Exceptions;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MultiCurrencyPocket.Services.ExchangeRateServices
{
    public class EcbEuropaExchangeRateService : IExchangeRateService
    {
        protected readonly string ApiReference;
        protected readonly int UpdateFrequency;
        private readonly IMemoryCache memoryCache;
        private readonly IHttpClientFactory httpClientFactory;

        protected const string CacheKey = "EcbEuropaCurrency:{0}";
        protected const string DefaultCurrency = "EUR";
        protected const decimal DefaultRate = 1.0m;
        
        protected static DateTime LastUpdate { get; set; }        
        
        public EcbEuropaExchangeRateService(IOptions<ExchangeServiceConfig> options, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory) 
        {
            ApiReference = options.Value.ApiReference;
            UpdateFrequency = options.Value.UpdateFrequency;
            this.memoryCache = memoryCache;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<decimal> GetRateAsync(string SourceCurrency, string DestinationCurrency)
        {
            if (SourceCurrency == DestinationCurrency)
                return 1.0m;
            var sourcerRate = await GetRateAsync(SourceCurrency);
            var destRate = await GetRateAsync(DestinationCurrency);

            return destRate / sourcerRate;
        }

        protected async Task<decimal> GetRateAsync(string currency) 
        {
            if (currency == DefaultCurrency)
                return DefaultRate;
            decimal resultRate = 0.0m;
            if (!memoryCache.TryGetValue(GetKey(currency), out resultRate)) 
            {
                await InitRatesCache();
                if (!memoryCache.TryGetValue(GetKey(currency), out resultRate))
                    throw new CurrencyRateNotFoundException();
            }
            return resultRate;
        }

        protected string GetKey(string id) => string.Format(CacheKey, id);

        protected async Task InitRatesCache() 
        {
            var xml = await LoadRatesXml();
            var xDoc = XDocument.Parse(xml);
            
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("ecb", @"http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            namespaceManager.AddNamespace("gesmes", @"http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            
            var items = xDoc.XPathSelectElements("//ecb:Cube/ecb:Cube/ecb:Cube", namespaceManager);

            var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            
            foreach (var item in items) 
            {
                var currency = item.Attribute("currency").Value;
                var rate = decimal.Parse(item.Attribute("rate").Value, style, CultureInfo.InvariantCulture);

                AddToCache(currency, rate);
            }
        }

        protected void AddToCache(string currency, decimal rate)
        {
            memoryCache.Set(GetKey(currency), rate, new DateTimeOffset(DateTime.Today.AddDays(1).ToUniversalTime()));
        }

        protected async Task<string> LoadRatesXml() 
        {
            //ToDo: Change to Named client
            var client = httpClientFactory.CreateClient();
            return await client.GetStringAsync(ApiReference);
        }

    }
}

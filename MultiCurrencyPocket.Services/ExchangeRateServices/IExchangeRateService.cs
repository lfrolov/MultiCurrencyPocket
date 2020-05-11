using System.Threading.Tasks;

namespace MultiCurrencyPocket.Services.ExchangeRateServices
{
    public interface IExchangeRateService
    {
        public Task<decimal> GetRateAsync(string SourceCurrency, string DestinationCurrency);
    }
}

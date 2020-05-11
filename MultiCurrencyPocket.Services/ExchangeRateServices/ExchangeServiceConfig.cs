namespace MultiCurrencyPocket.Services.ExchangeRateServices
{
    public class ExchangeServiceConfig
    {
        /// <summary>
        /// Url to rate of exchange service
        /// </summary>
        public string ApiReference { get; set; }

        /// <summary>
        /// How often update data from service in seconds
        /// </summary>
        public int UpdateFrequency { get; set; }
    }
}

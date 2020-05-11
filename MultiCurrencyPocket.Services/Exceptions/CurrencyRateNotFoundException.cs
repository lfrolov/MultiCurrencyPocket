using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCurrencyPocket.Services.Exceptions
{
    public class CurrencyRateNotFoundException : Exception
    {
        private const string DefaultMessage = "Currency rate is not found or wrong currency name.";
        public CurrencyRateNotFoundException() : base(DefaultMessage)
        {
        }
    }
}

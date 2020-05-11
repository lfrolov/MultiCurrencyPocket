using System;

namespace MultiCurrencyPocket.Services.Exceptions
{
    public class InsufficientFundException : Exception
    {
        private const string DefaultMessage = "Account has insufficient funds.";
        public InsufficientFundException() : base(DefaultMessage)
        {
        }
    }
}

using System;

namespace MultiCurrencyPocket.Services.Exceptions
{
    public class ConcurrencyException : Exception
    {
        private const string DefaultMessage = "Concurrency exception occured. Please update data and try again.";
        public ConcurrencyException() : base(DefaultMessage) 
        {
        }
    }
}

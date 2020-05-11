using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCurrencyPocket.Services.Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public const string DefaultErrorMessage = "Wrong account number or pincode.";
        public const string WrongCurrencyMessage = "Hoelder doen't have {0} currency account";

        public AccountNotFoundException() : base(DefaultErrorMessage)
        {
        }

        public AccountNotFoundException(string message) : base(message)
        { }

        public AccountNotFoundException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

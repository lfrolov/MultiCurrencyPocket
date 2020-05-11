using System;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public class DepositCurrencyDTO : CurrencyDTO
    {
        [Range(0, 1e6)]
        public decimal Sum { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public class WithdrawCurrencyDTO : CurrencyDTO
    {
        [Range(0, 1e6)]
        public decimal Sum { get; set; }
    }
}

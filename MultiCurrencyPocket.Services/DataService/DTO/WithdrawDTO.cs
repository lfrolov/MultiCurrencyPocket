using System;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public class WithdrawDTO : MasterDTO
    {
        [Required]
        public string Currency { get; set; }

        [Range(0, 1e6)]
        public decimal Sum { get; set; }
    }
}

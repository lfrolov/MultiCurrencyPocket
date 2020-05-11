using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public class StatusResultDTO
    {
        public IEnumerable<DepositItemDTO> Deposits { get; set; }
    }

    public class DepositItemDTO 
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string Currency { get; set; }

        public decimal Sum { get; set; }

    }

}

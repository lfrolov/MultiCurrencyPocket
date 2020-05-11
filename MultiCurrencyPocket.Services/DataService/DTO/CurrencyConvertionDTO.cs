using System;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public class CurrencyConvertionDTO : MasterDTO
    {
        [Required]
        [RegularExpression("[A-Z]{3}")]
        public string SourceCurrency { get; set; }

        [Required]
        [RegularExpression("[A-Z]{3}")]
        public string DestinationCurrency { get; set; }

        [Range(0, 1e6)]
        public decimal Sum { get; set; }
    }
}

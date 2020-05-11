using System;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public abstract class MasterDTO
    {
        [Required]
        [RegularExpression(@"\d{6}")]
        public string MasterAccount { get; set; }

        [Required]
        [RegularExpression(@"\d{4}")]
        public string PinCode { get; set; }
    }
}

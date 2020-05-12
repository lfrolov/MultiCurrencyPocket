using System;
using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public abstract class MasterDTO
    {
        [Required]
        [RegularExpression(@"\d{6}", ErrorMessage = "MasterAccount should be in format ######")]
        public string MasterAccount { get; set; }

        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "PinCode should be in format ####")]
        public string PinCode { get; set; }
    }
}

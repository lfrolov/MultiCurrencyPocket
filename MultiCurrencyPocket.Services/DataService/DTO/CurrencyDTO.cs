using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public abstract class CurrencyDTO
    {
        [Required]
        [RegularExpression(@"\d{6}", ErrorMessage = "CurrencyAccount should be in format ######")]
        public string CurrencyAccount { get; set; }

        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "PinCode should be in format ####")]
        public string PinCode { get; set; }
    }
}

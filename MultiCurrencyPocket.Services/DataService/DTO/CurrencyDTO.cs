using System.ComponentModel.DataAnnotations;

namespace MultiCurrencyPocket.Services.DataService.DTO
{
    public abstract class CurrencyDTO
    {
        [Required]
        [RegularExpression(@"\d{6}")]
        public string CurrencyAccount { get; set; }

        [Required]
        [RegularExpression(@"\d{4}")]
        public string PinCode { get; set; }
    }
}

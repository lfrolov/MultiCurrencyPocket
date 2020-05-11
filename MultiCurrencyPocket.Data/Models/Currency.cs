using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiCurrencyPocket.Data.Models
{
    public class Currency
    {
        [Key]
        [Required]
        [RegularExpression(@"[A-Z]{3}")]        
        [Column("Currency")]
        public string ShortCurrency { get; set; }

        public int Code { get; set; }

        public string FullCurrencyName { get; set; }

        [StringLength(50)]
        public string Country { get; set; }
    }
}

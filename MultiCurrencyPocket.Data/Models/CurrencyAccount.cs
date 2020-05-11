using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MultiCurrencyPocket.Data.Models
{
    public class CurrencyAccount
    {
        [Key]
        public int Id { get; set; }

        [RegularExpression(@"\d{6}")]
        public string Number { get; set; }

        public decimal Debit { get; set; }

        public string Currency { get; set; }

        //[ForeignKey("Currency")]
        //public Currency Currency { get; set; }

        public int PocketHolderId { get; set;}

        [ForeignKey("PocketHolderId")]
        public PocketHolder Holder { get; set; }
    }
}

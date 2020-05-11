using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiCurrencyPocket.Data.Models
{
    public class PocketHolder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //TODO: Need to implement some Digit check algorithm like Luhn, CRC or use CreditCardAttribute
        [Required]
        [RegularExpression(@"\d{6}")]
        public string MasterAccount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; } 

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        
        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "tinyint")]
        public GenderCode Gender { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PINCode { get; set; }

        public List<CurrencyAccount> Accounts { get; set; }

    }

    public enum GenderCode : byte
    {
        Female = 0,
        Male = 1
    }
}

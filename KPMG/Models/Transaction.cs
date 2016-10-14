using KPMG.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KPMG.Models
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [Column(TypeName = "ntext")]
        [MinLength(0, ErrorMessage = "The Account value cannot be empty.")]
        public string Account { get; set; }
        [Required]
        [Column(TypeName = "ntext")]
        [MinLength(0, ErrorMessage = "The Description value cannot be empty.")]
        public string Description { get; set; }
        [ValidateCurrency(ErrorMessage = ISOCurrencySymbols.CURRENCY_ERROR)]
        [StringLength(3, MinimumLength = 3)]
        [Column(TypeName = "char")]
        public string CurrencyCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
        
        public Transaction()
        {
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Account, Description, CurrencyCode, Amount);
        }
    }

    // Validation for currency code. It checks against the static list of valid symbols
    public class ValidateCurrencyAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var symbol = value as string;
            if (symbol == null)
                return false;

            if (!ISOCurrencySymbols.Instance.IsISO4217(symbol))
                return false;

            return true;
        }
    }
}
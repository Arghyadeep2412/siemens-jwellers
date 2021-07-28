using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.Models.CodeModels
{
    public class CustomDataModels
    {
    }

    public class NewRateDetails
    {
        public int? RateId { get; set; }
        public string ItemType { get; set; }
        public decimal? RateAmount { get; set; }
        public string Unit { get; set; }
        public string Currency { get; set; }
    }

    public class CustomerDetails
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public byte? CustomerType { get; set; } = Globals.NormalCustomer;
    }

    public class NewLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ResetLoginCreds : NewLogin
    {
        public string Email { get; set; }
    }
    public class NewCustomer : CustomerDetails
    {
        
    }

    public class NewInvoice
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public string ItemType { get; set; } // gold
        public string Currency { get; set; } // inr
        public string WeightUnit { get; set; } // grams
        public decimal? Weight { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public byte? PaymentStatus { get; set; } = Globals.UnPaid;
    }
}

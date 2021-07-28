using System;
using System.Collections.Generic;

#nullable disable

namespace JewelleryStore.Models.DBModels
{
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public string ItmeType { get; set; }
        public decimal Rate { get; set; }
        public string Currency { get; set; }
        public string PricePerUnit { get; set; }
        public decimal Weight { get; set; }
        public string WeightUnit { get; set; }
        public decimal ActualPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? PaymentStatus { get; set; }

        public virtual Customer User { get; set; }
    }
}

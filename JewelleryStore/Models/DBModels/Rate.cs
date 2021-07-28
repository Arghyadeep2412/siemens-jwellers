using System;
using System.Collections.Generic;

#nullable disable

namespace JewelleryStore.Models.DBModels
{
    public partial class Rate
    {
        public int RateId { get; set; }
        public string ItemType { get; set; }
        public decimal RateAmount { get; set; }
        public string Unit { get; set; }
        public string Currency { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

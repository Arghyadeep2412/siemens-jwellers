using System;
using System.Collections.Generic;

#nullable disable

namespace JewelleryStore.Models.DBModels
{
    public partial class Customer
    {
        public Customer()
        {
            Invoices = new HashSet<Invoice>();
            UsersLoginCreds = new HashSet<UsersLoginCred>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }
        public byte? CustomerType { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<UsersLoginCred> UsersLoginCreds { get; set; }
    }
}

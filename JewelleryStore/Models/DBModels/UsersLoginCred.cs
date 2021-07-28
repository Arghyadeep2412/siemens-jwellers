using System;
using System.Collections.Generic;

#nullable disable

namespace JewelleryStore.Models.DBModels
{
    public partial class UsersLoginCred
    {
        public int UserLoginId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Customer User { get; set; }
    }
}

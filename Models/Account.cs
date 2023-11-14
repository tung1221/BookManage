using System;
using System.Collections.Generic;

namespace BookManage.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public Customer()
        {
            this.Products = new HashSet<Product>();
        }
    
        public int CustomerID { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string HomeAdress { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string ContactNumber { get; set; }
        public string UserImage { get; set; }
    
        public virtual ICollection<Product> Products { get; set; }
    }
}

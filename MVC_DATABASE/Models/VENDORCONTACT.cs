//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_DATABASE.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VENDORCONTACT
    {
        public string Id { get; set; }
        public string CONTACTNAME { get; set; }
        public string CONTACTPHONE { get; set; }
        public string CONTACTEMAIL { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}

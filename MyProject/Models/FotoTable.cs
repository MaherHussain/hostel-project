//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FotoTable
    {
        public int FotoID { get; set; }
        public int ItemID_Fk { get; set; }
        public string FotoNavn { get; set; }
    
        public virtual VaerlseTable VaerlseTable { get; set; }
    }
}

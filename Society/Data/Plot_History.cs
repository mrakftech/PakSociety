//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Society.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Plot_History
    {
        public int History_ID { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> History_No { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone_Number { get; set; }
        public Nullable<System.DateTime> H_Date { get; set; }
        public Nullable<int> Plot_ID { get; set; }
        public string CNIC { get; set; }
    
        public virtual Plot Plot { get; set; }
    }
}
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
    
    public partial class Attendence
    {
        public int ID { get; set; }
        public Nullable<int> Employee_ID { get; set; }
        public string Employee_Name { get; set; }
        public Nullable<System.TimeSpan> IN_Time { get; set; }
        public Nullable<System.TimeSpan> OUT_Time { get; set; }
        public System.DateTime Attendence_date { get; set; }
        public Nullable<bool> Attendence_Status { get; set; }
        public Nullable<double> Over_Time { get; set; }
        public string Shift { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
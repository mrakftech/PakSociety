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
    
    public partial class MemberShipFee
    {
        public int Id { get; set; }
        public Nullable<int> member_id { get; set; }
        public Nullable<int> Payment_ID { get; set; }
        public Nullable<double> Previous_MS_Fee { get; set; }
        public Nullable<double> Current_MS_Fee { get; set; }
        public Nullable<System.DateTime> Payment_Date { get; set; }
        public string Manual_Voucher_Name { get; set; }
        public Nullable<double> MS_Amount { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Payment_Plan Payment_Plan { get; set; }
    }
}
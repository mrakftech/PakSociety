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
    
    public partial class File_Transfer
    {
        public int ID { get; set; }
        public string EB_No { get; set; }
        public string From_CNIC { get; set; }
        public string To_CNIC { get; set; }
        public string From_Name { get; set; }
        public string To_Name { get; set; }
        public System.DateTime Date { get; set; }
        public string Prefix { get; set; }
        public byte[] fromImage { get; set; }
        public byte[] toImage { get; set; }
        public Nullable<int> memberId { get; set; }
        public byte[] fromThumb { get; set; }
        public byte[] toThumb { get; set; }
        public byte[] groupPhoto { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
    
        public virtual Member Member { get; set; }
    }
}

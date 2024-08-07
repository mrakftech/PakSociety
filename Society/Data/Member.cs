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
    
    public partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            this.Dealer_Commision = new HashSet<Dealer_Commision>();
            this.File_Transfer = new HashSet<File_Transfer>();
            this.Ledger_Account = new HashSet<Ledger_Account>();
            this.MemberShipFees = new HashSet<MemberShipFee>();
            this.memberImages = new HashSet<memberImage>();
            this.Parties = new HashSet<Party>();
            this.Plots = new HashSet<Plot>();
        }
    
        public int Member_ID { get; set; }
        public System.DateTime Application_Date { get; set; }
        public string CNIC { get; set; }
        public string Applicant_Start_Text { get; set; }
        public string Applicant_Name { get; set; }
        public string Father_Husband_Start_Text { get; set; }
        public string Father_Husband_Name { get; set; }
        public string Occupation { get; set; }
        public string Present_Postel_Address { get; set; }
        public string Permenent_Postel_Address { get; set; }
        public string Office_No { get; set; }
        public string Cell_No { get; set; }
        public string Email { get; set; }
        public string Nominee_Name { get; set; }
        public string Nominee_CNIC { get; set; }
        public string Relation { get; set; }
        public string Nominee_Phone_No { get; set; }
        public string Nominee_Address { get; set; }
        public byte[] Profile_Img { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> Refund_Check { get; set; }
        public string Dealer_Name { get; set; }
        public Nullable<int> Employee_id { get; set; }
        public string notes { get; set; }
        public string ILNo { get; set; }
        public byte[] thumb_img { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string Country { get; set; }
        public string NomineeFather { get; set; }
        public string NomineeFatherStartText { get; set; }
        public string MemberShipNumber { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dealer_Commision> Dealer_Commision { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<File_Transfer> File_Transfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ledger_Account> Ledger_Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemberShipFee> MemberShipFees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<memberImage> memberImages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Party> Parties { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Plot> Plots { get; set; }
    }
}

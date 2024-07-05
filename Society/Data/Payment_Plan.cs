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
    
    public partial class Payment_Plan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Payment_Plan()
        {
            this.Development_Charges = new HashSet<Development_Charges>();
            this.Installments = new HashSet<Installment>();
            this.MemberShipFees = new HashSet<MemberShipFee>();
            this.paymentVouchers = new HashSet<paymentVoucher>();
        }
    
        public int Payment_ID { get; set; }
        public Nullable<double> Gross_Price_Plot { get; set; }
        public Nullable<double> Extra_Charge { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> Net_Price_Plot { get; set; }
        public Nullable<double> total_downpayment { get; set; }
        public Nullable<double> Pocession_Payment { get; set; }
        public Nullable<int> Plot_ID { get; set; }
        public string description { get; set; }
        public Nullable<double> Outstanding { get; set; }
        public Nullable<double> development_charges_per_marla { get; set; }
        public Nullable<double> memberShipFee { get; set; }
        public Nullable<double> RatePerSqFit { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Development_Charges> Development_Charges { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Installment> Installments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemberShipFee> MemberShipFees { get; set; }
        public virtual Plot Plot { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<paymentVoucher> paymentVouchers { get; set; }
    }
}
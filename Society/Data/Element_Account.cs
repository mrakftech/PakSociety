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
    
    public partial class Element_Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Element_Account()
        {
            this.Account_Setting = new HashSet<Account_Setting>();
            this.Control_Account = new HashSet<Control_Account>();
            this.Ledger_Account = new HashSet<Ledger_Account>();
        }
    
        public int ID { get; set; }
        public Nullable<int> Element_Account_Code { get; set; }
        public string Account_Title { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account_Setting> Account_Setting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Control_Account> Control_Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ledger_Account> Ledger_Account { get; set; }
    }
}

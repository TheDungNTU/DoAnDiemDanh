//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAnDiemDanh.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DIEMDANH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DIEMDANH()
        {
            this.CTDDs = new HashSet<CTDD>();
            this.CTDD_GV = new HashSet<CTDD_GV>();
        }
    
        public int MaDD { get; set; }
        public Nullable<int> MaNMH { get; set; }
        public Nullable<System.DateTime> NgayDiemDanh { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTDD> CTDDs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTDD_GV> CTDD_GV { get; set; }
        public virtual NHOMMONHOC NHOMMONHOC { get; set; }
    }
}

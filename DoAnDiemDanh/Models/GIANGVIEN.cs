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
    
    public partial class GIANGVIEN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GIANGVIEN()
        {
            this.CTDD_GV = new HashSet<CTDD_GV>();
            this.DIEMDANH_T_GV = new HashSet<DIEMDANH_T_GV>();
            this.TAIKHOANGIANGVIENs = new HashSet<TAIKHOANGIANGVIEN>();
            this.HINHANH_GV = new HashSet<HINHANH_GV>();
            this.NHOMMONHOCs = new HashSet<NHOMMONHOC>();
        }
    
        public int MaGV { get; set; }
        public string TenGV { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Avatar { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public Nullable<int> MaKhoa { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTDD_GV> CTDD_GV { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIEMDANH_T_GV> DIEMDANH_T_GV { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAIKHOANGIANGVIEN> TAIKHOANGIANGVIENs { get; set; }
        public virtual KHOA KHOA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HINHANH_GV> HINHANH_GV { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NHOMMONHOC> NHOMMONHOCs { get; set; }
    }
}

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
    
    public partial class DIEMDANH_T_GV
    {
        public int MaDD { get; set; }
        public Nullable<int> MaGV { get; set; }
        public Nullable<System.DateTime> Ngay { get; set; }
        public Nullable<System.TimeSpan> Gio { get; set; }
    
        public virtual GIANGVIEN GIANGVIEN { get; set; }
    }
}

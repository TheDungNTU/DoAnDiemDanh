﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FACE_RECOGNITION_V2Entities : DbContext
    {
        public FACE_RECOGNITION_V2Entities()
            : base("name=FACE_RECOGNITION_V2Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CAMERA> CAMERAs { get; set; }
        public virtual DbSet<CTDD> CTDDs { get; set; }
        public virtual DbSet<CTDD_GV> CTDD_GV { get; set; }
        public virtual DbSet<DIEMDANH> DIEMDANHs { get; set; }
        public virtual DbSet<DIEMDANH_T_GV> DIEMDANH_T_GV { get; set; }
        public virtual DbSet<DIEMDANH_T_SV> DIEMDANH_T_SV { get; set; }
        public virtual DbSet<GIANGVIEN> GIANGVIENs { get; set; }
        public virtual DbSet<HINHANH_GV> HINHANH_GV { get; set; }
        public virtual DbSet<HINHANH_SV> HINHANH_SV { get; set; }
        public virtual DbSet<KHIEUNAI> KHIEUNAIs { get; set; }
        public virtual DbSet<KHOA> KHOAs { get; set; }
        public virtual DbSet<LICHGIANGDAY> LICHGIANGDAYs { get; set; }
        public virtual DbSet<LOP> LOPs { get; set; }
        public virtual DbSet<MONHOC> MONHOCs { get; set; }
        public virtual DbSet<NGAYLE> NGAYLEs { get; set; }
        public virtual DbSet<NHOMMONHOC> NHOMMONHOCs { get; set; }
        public virtual DbSet<PHONGHOC> PHONGHOCs { get; set; }
        public virtual DbSet<QUYEN> QUYENs { get; set; }
        public virtual DbSet<SINHVIEN> SINHVIENs { get; set; }
        public virtual DbSet<TAIKHOANGIANGVIEN> TAIKHOANGIANGVIENs { get; set; }
        public virtual DbSet<TAIKHOANSINHVIEN> TAIKHOANSINHVIENs { get; set; }
    }
}

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
    
    public partial class FACE_RECOGNITIONEntities : DbContext
    {
        public FACE_RECOGNITIONEntities()
            : base("name=FACE_RECOGNITIONEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ADMIN> ADMINs { get; set; }
        public virtual DbSet<CTDD> CTDDs { get; set; }
        public virtual DbSet<DIEMDANH> DIEMDANHs { get; set; }
        public virtual DbSet<DIEMDANH_T> DIEMDANH_T { get; set; }
        public virtual DbSet<GIANGVIEN> GIANGVIENs { get; set; }
        public virtual DbSet<HINHANH> HINHANHs { get; set; }
        public virtual DbSet<KHOA> KHOAs { get; set; }
        public virtual DbSet<LOP> LOPs { get; set; }
        public virtual DbSet<MONHOC> MONHOCs { get; set; }
        public virtual DbSet<SINHVIEN> SINHVIENs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}

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
    
    public partial class CAMERA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CAMERA()
        {
            this.PHONGHOCs = new HashSet<PHONGHOC>();
        }
    
        public int MaCamera { get; set; }
        public string TenCamera { get; set; }
        public string RTSP { get; set; }
        public string URL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PHONGHOC> PHONGHOCs { get; set; }
    }
}

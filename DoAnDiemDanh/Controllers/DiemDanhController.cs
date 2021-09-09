using DoAnDiemDanh.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin, GiangVien")]
    //[Authorize]
    public class DiemDanhController : Controller
    {
        public int getMaGV()
        {
            HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                if (!string.IsNullOrEmpty(authCookie.Value))
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    string str = authTicket.UserData;
                    string[] subs = str.Split(',');
                    return Int32.Parse(subs[0]);
                }
            }
            return -1;
        }

        public string getQuyen()
        {
            HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                if (!string.IsNullOrEmpty(authCookie.Value))
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    string str = authTicket.UserData;
                    string[] subs = str.Split(',');
                    return subs[2];
                }
            }
            return "";
        }


        private BaseModel db = new BaseModel();

        public ActionResult Index()
        {
            
            ViewBag.MONHOC = db.Entity.MONHOCs;
            return View();
        }


        public ActionResult DiemDanhGV()
        {
           
            ViewBag.MONHOC = db.Entity.MONHOCs;          
            return View();
        }

        public ActionResult XemDiemDanh(int MaMH)
        { 
            ViewBag.MonHoc = db.Entity.MONHOCs.Where(s => s.MaMH == MaMH);
            var nHOMMONHOCs = db.Entity.NHOMMONHOCs.Where(s => s.MaMH == MaMH);
            return View(nHOMMONHOCs.ToList());
        }


        public ActionResult XemChiTietDiemDanh(int MaNMH)
        {
            var NHOMMONHOC = db.Entity.NHOMMONHOCs.Where(s => s.MaNMH == MaNMH);
            return View(NHOMMONHOC.ToList());
        }

        public ActionResult XemChiTietDiemDanhGV(int MaNMH)
        {
            var NHOMMONHOC = db.Entity.NHOMMONHOCs.Where(s => s.MaNMH == MaNMH);
            return View(NHOMMONHOC.ToList());
        }

        public ActionResult XemDiemDanhGV(int MaMH)
        {
            ViewBag.MONHOC = db.Entity.MONHOCs.Where(s => s.MaMH == MaMH);
            var nHOMMONHOCs = db.Entity.NHOMMONHOCs.Where(s => s.MaMH == MaMH);
            return View(nHOMMONHOCs.ToList());
        }

        public JsonResult GetSinhVien(string MaNMH, string d1)
        {
            var manmh = Int32.Parse(MaNMH);
            db.Entity.Configuration.ProxyCreationEnabled = false;
            DateTime date = DateTime.ParseExact(d1, "dd/MM/yyyy", null);
            var data = from diemdanh in db.Entity.DIEMDANHs
                       join ctdd in db.Entity.CTDDs on diemdanh.MaDD equals ctdd.MaDD
                       join sinhvien in db.Entity.SINHVIENs on ctdd.MaSV equals sinhvien.MaSV
                       join lop in db.Entity.LOPs on sinhvien.MaLop equals lop.MaLop
                       where diemdanh.MaNMH == manmh && diemdanh.NgayDiemDanh == date
                       select new {MaSV = sinhvien.MaSV , TenLop = lop.TenLop, TenSV = sinhvien.TenSV, ThoiGianVao = ctdd.ThoiGianVao, ThoiGianRa = ctdd.ThoiGianRa, TrangThai = ctdd.TTDD, CoPhep = ctdd.VangCoPhep, KhieuNai = ctdd.MaKN};
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiemDanhGV(int MaNMH)
        {
            db.Entity.Configuration.ProxyCreationEnabled = false;
            var data = from diemdanh in db.Entity.DIEMDANHs
                       join ctdd_gv in db.Entity.CTDD_GV on diemdanh.MaDD equals ctdd_gv.MaDD
                       where diemdanh.MaNMH == MaNMH
                       select new { Ngay = diemdanh.NgayDiemDanh, ThoiGianVao = ctdd_gv.ThoiGianVao, ThoiGianRa = ctdd_gv.ThoiGianRa, TrangThai = ctdd_gv.TTDD };
            var list = new List<DDGV>();
           
            CultureInfo vn = new CultureInfo("vi-VN");
            foreach (var item in data)
            {
                var dd = new DDGV();
                DateTime dt = (DateTime)item.Ngay;

                dd.Ngay = Convert.ToDateTime(dt).ToString("dd/MM/yyyy");

                dd.Thu = vn.DateTimeFormat.GetDayName(dt.DayOfWeek);

                if(item.ThoiGianRa != null)
                {
                    TimeSpan tgr = (TimeSpan)item.ThoiGianRa;
                    dd.ThoiGianRa = tgr.ToString(@"hh\:mm");
                }


                if (item.ThoiGianVao != null)
                {
                    TimeSpan tgv = (TimeSpan)item.ThoiGianVao;
                    dd.ThoiGianVao = tgv.ToString(@"hh\:mm");
                }
               

                dd.TrangThai = (bool)item.TrangThai;

                list.Add(dd);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public class DDGV
        {
            public string Ngay { get; set; }
            public string Thu { get; set; }
            public  string ThoiGianVao { get; set; }
            public string ThoiGianRa { get; set; }
            public bool TrangThai { get; set; }

        }
    }
}
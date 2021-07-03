using DoAnDiemDanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin, GiangVien")]
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

        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();
        // GET: DiemDanh
 
        public ActionResult Index()
        {
            if (getQuyen() == "Admin")
            {
                ViewBag.MONHOC = db.MONHOCs;
            }
            else
            {
                int MaGV = getMaGV();
                ViewBag.MONHOC = db.MONHOCs.Where(s => s.MaGV == MaGV);
            }
            return View();
        }

        public ActionResult XemDiemDanh(int MaMH)
        {

            ViewBag.MONHOC = db.MONHOCs.Where(s => s.MaMH == MaMH);
            return View();
        }

        public JsonResult GetSinhVien(int MaMH, string d1)
        {
            db.Configuration.ProxyCreationEnabled = false;
            DateTime date = DateTime.ParseExact(d1, "dd/MM/yyyy", null);
            var data = from diemdanh in db.DIEMDANHs
                       join ctdd in db.CTDDs on diemdanh.MaDD equals ctdd.MaDD
                       join sinhvien in db.SINHVIENs on ctdd.MaSV equals sinhvien.MaSV
                       join lop in db.LOPs on sinhvien.MaLop equals lop.MaLop
                       where diemdanh.MaMH == MaMH && diemdanh.NgayDiemDanh == date
                       select new {MaSV = sinhvien.MaSV , TenLop = lop.TenLop, TenSV = sinhvien.TenSV, ThoiGianVao = ctdd.ThoiGianVao, ThoiGianRa = ctdd.ThoiGianRa, TrangThai = ctdd.TTDD, CoPhep = ctdd.VangCoPhep, KhieuNai = ctdd.MaKN};
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    public class QuanLyKhieuNaiController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        
        public ActionResult AlertPartial()
        {
            var lstKhieuNai = db.KHIEUNAIs.Where(s => s.DaXem == false);
            return PartialView(lstKhieuNai);
        }

        public ActionResult Details(int? id)
        {
            ViewBag.id = id;

            var diemdanh = (from ctdd in db.CTDDs
                       where ctdd.MaKN == id
                       join dd in db.DIEMDANHs on ctdd.MaDD equals dd.MaDD
                       select dd).SingleOrDefault();
        

            ViewBag.MONHOC = db.MONHOCs.Where(s => s.MaMH == diemdanh.MaMH);



            return View();
        }

        public ActionResult SubmitKhieuNai(int MaDD, int MaSV,int MaMH ,int TrangThaiVang)
        {
            var ctdd = db.CTDDs.SingleOrDefault(s => s.MaDD == MaDD && s.MaSV == MaSV);
            if(TrangThaiVang == 0)
            {
                ctdd.VangCoPhep = false;
            }
            else
            {
                ctdd.VangCoPhep = true;
            }
           
            db.Entry(ctdd).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("XemDiemDanh","DiemDanh", new {MaMH = MaMH});
        }

        [HttpGet]
        public JsonResult GetDetails(int? id)
        {
            int MaKN = (int)id;
            var KhieuNai = db.KHIEUNAIs.SingleOrDefault(_ => _.MaKN == id);
            CultureInfo vn = new CultureInfo("vi-VN");
            var tt = (from ctdd in db.CTDDs
                      where ctdd.MaKN == KhieuNai.MaKN
                      join dd in db.DIEMDANHs on ctdd.MaDD equals dd.MaDD
                      join sv in db.SINHVIENs on ctdd.MaSV equals sv.MaSV
                     
                      select new
                      {
                          MaSV = sv.MaSV,
                          TenSV = sv.TenSV,
                          NgayDD = dd.NgayDiemDanh,
                          TenLop = sv.LOP.TenLop,
                          MaDD = ctdd.MaDD,
                          MaMH = dd.MaMH
                      }).SingleOrDefault();

            DateTime dt = (DateTime)tt.NgayDD;
            var thu = vn.DateTimeFormat.GetDayName(dt.DayOfWeek);
            DateTime dt1 = (DateTime)KhieuNai.NgayGui;
            var ngaydd = Convert.ToDateTime(dt).ToString("dd/MM/yyyy");
            var ngaykn = Convert.ToDateTime(dt1).ToString("dd/MM/yyyy");

            var data = new
            {
                MaSV = tt.MaSV,
                TenSV = tt.TenSV,
                TenLop = tt.TenLop,
                NgayDD = ngaydd,
                NgayKN = ngaykn,
                HinhAnh = KhieuNai.TenHA,
                NoiDung = KhieuNai.NoiDung,
                Thu = thu,
                MaDD = tt.MaDD,
                MaMH = tt.MaMH
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

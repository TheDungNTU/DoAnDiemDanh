using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    
    public class QuanLyMonHocController : Controller
    {

        private BaseModel db = new BaseModel();
        // GET: QuanLyMonHoc

        [Authorize(Roles = "Admin, GiangVien")]
        public IEnumerable<DateTime> EachDay(DateTime d1, DateTime d2)
        {
            for (var day = d1.Date; day.Date <= d2.Date; day = day.AddDays(1))
                yield return day;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var mONHOCs = db.Entity.MONHOCs;
            return View(mONHOCs.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaMH,TenMH,SoTC,SoNgayVang")] MONHOC mONHOC)
        {

            var Khoa = db.Entity.MONHOCs.SingleOrDefault(s => s.TenMH == mONHOC.TenMH);
            if (Khoa == null)
            {
                db.Entity.MONHOCs.Add(mONHOC);
                db.Entity.SaveChanges();
                return Json(mONHOC, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        // GET: QuanLyMonHoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONHOC mONHOC = db.Entity.MONHOCs.Find(id);
            if (mONHOC == null)
            {
                return HttpNotFound();
            }
            return View(mONHOC);
        }

        [Authorize(Roles = "Admin, GiangVien")]
        public ActionResult DangKyMonHoc()
        {
            ViewBag.MaMH = db.Entity.MONHOCs;
            ViewBag.Khoa_Filter = db.Entity.KHOAs;
            ViewBag.Lop_Filter = db.Entity.LOPs;
            return View();
        }

        [Authorize(Roles = "Admin, GiangVien")]
        [HttpGet]
        public JsonResult GetThongTinMonHoc(int MaMH, int MaNMH)
        {

            var MonHoc = db.Entity.MONHOCs.SingleOrDefault(_ => _.MaMH == MaMH);
            var NhomMonHoc = db.Entity.NHOMMONHOCs.SingleOrDefault(_ => _.MaNMH == MaNMH);
            var SinhVienDK = from sv in db.Entity.SINHVIENs
                             where sv.NHOMMONHOCs.Any(mh => mh.MaNMH == MaNMH)
                             select sv.MaSV;

            var SinhVien = from sv in db.Entity.SINHVIENs
                           where !SinhVienDK.Contains(sv.MaSV)
                           select new { MaSV = sv.MaSV, TenSV = sv.TenSV, TenKhoa = sv.KHOA.TenKhoa, TenLop = sv.LOP.TenLop };

            JsonResult categoryJson = new JsonResult();
            categoryJson.Data = SinhVien;
            var data = new
            {
                TenMH = MonHoc.TenMH,
                TenGV = NhomMonHoc.GIANGVIEN.TenGV,
                SoTC = MonHoc.SoTC,
                TenPhongHoc = NhomMonHoc.PHONGHOC.TenPhongHoc,
                SinhVien = categoryJson,
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNhomMonHoc(int id)
        {
            db.Entity.Configuration.ProxyCreationEnabled = false;
            var NHOMMONHOC = db.Entity.NHOMMONHOCs.Where(_ => _.MaMH == id).ToList();
            return Json(NHOMMONHOC, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, GiangVien")]
        [HttpPost]
        public JsonResult DangKyMonHoc(string MaNMH, List<string> data)
        {
            string sqlconnectStr = ConfigurationManager.ConnectionStrings["ASPNETConnectionString"].ToString();
            var cnn = new SqlConnection(sqlconnectStr);
            cnn.Open();
            foreach(var item in data)
            {
                var command = new SqlCommand($"INSERT INTO DANGKYMONHOC VALUES ({item},{MaNMH})", cnn);
                command.ExecuteNonQuery();

                int manmh = Int32.Parse(MaNMH);
                int masv = Int32.Parse(item);

                var DiemDanh = db.Entity.DIEMDANHs.Where(_ => _.MaNMH == manmh).ToList();
                var CTDD = new CTDD();
                foreach (var item1 in DiemDanh)
                {
                    command = new SqlCommand($"INSERT INTO dbo.CTDD ( MaDD, MaSV) VALUES  ( {item1.MaDD},{masv})", cnn);
                    command.ExecuteNonQuery();
                }
            }

            cnn.Close();
            return Json(MaNMH, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin")]
        //// GET: QuanLyMonHoc/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MONHOC mONHOC = db.Entity.MONHOCs.Find(id);
        //    if (mONHOC == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MaGV = new SelectList(db.Entity.GIANGVIENs, "MaGV", "TenGV", mONHOC.MaGV);
        //    return View(mONHOC);
        //}

        // POST: QuanLyMonHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "MaMH,TenMH,SoTC,NgayBD,NgayKT,ThoiGianBDGD,ThoiGianKTGD,MaGV")] MONHOC mONHOC)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entity.Entry(mONHOC).State = EntityState.Modified;
        //        db.Entity.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MaGV = new SelectList(db.Entity.GIANGVIENs, "MaGV", "TenGV", mONHOC.MaGV);
        //    return View(mONHOC);
        //}

        [Authorize(Roles = "Admin")]
        // POST: QuanLyMonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            MONHOC mONHOC = db.Entity.MONHOCs.Find(id);
            var data = new
            {
                id = id,
                TenMH = mONHOC.TenMH
            };
            db.Entity.MONHOCs.Remove(mONHOC);
            db.Entity.SaveChanges();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Entity.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

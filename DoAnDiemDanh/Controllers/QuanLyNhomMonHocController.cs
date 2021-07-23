using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    public class QuanLyNhomMonHocController : Controller
    {
        private FACE_RECOGNITION_V2Entities db = new FACE_RECOGNITION_V2Entities();

        public IEnumerable<DateTime> EachDay(DateTime d1, DateTime d2)
        {
            for (var day = d1.Date; day.Date <= d2.Date; day = day.AddDays(1))
                yield return day;
        }


        // GET: QuanLyNhomMonHoc
        public ActionResult Index(int MaMH)
        {
            ViewBag.MonHoc = db.MONHOCs.Where(s => s.MaMH == MaMH);
            ViewBag.GiangVien = db.GIANGVIENs;
            ViewBag.PhongHoc = db.PHONGHOCs;
            var nHOMMONHOCs = db.NHOMMONHOCs.Where(s => s.MaMH == MaMH);
            return View(nHOMMONHOCs.ToList());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaNMH,MaMH,MaGV,MaPhongHoc,NgayBD,ThoiGianBD,ThoiGianKT")] NHOMMONHOC nHOMMONHOC, List<int> TKB)
        {
            if (ModelState.IsValid)
            {
                var MH = db.MONHOCs.Single(s => s.MaMH == nHOMMONHOC.MaMH);
                int i = 0;
                var day = (DateTime)nHOMMONHOC.NgayBD;
                List<DateTime> listdate = new List<DateTime>();

                while (i < 15*(int)MH.SoTC)
                {
                    var ngay = (int)day.DayOfWeek;
                    if (TKB.Contains(ngay))
                    {
                        listdate.Add(day);
                        i++;
                    }
                    day = day.AddDays(1);
                }

                nHOMMONHOC.NgayKT = day.AddDays(-1);
                db.NHOMMONHOCs.Add(nHOMMONHOC);
                db.SaveChanges();

                LICHGIANGDAY tkb = new LICHGIANGDAY();
                tkb.MaNMH = nHOMMONHOC.MaNMH;

                foreach (var item in TKB)
                {
                    if (item == 1)
                        tkb.ThuHai = true;  
                    else if (item == 2)
                        tkb.ThuBa = true;
                    else if (item == 3)
                        tkb.ThuTu = true;
                    else if (item == 4)
                        tkb.ThuNam = true;
                    else if (item == 5)
                        tkb.ThuSau = true;
                    else
                        tkb.ThuBay = true;
                }

                if (tkb.ThuHai == null) tkb.ThuHai = false;
                if (tkb.ThuBa == null) tkb.ThuBa = false;
                if (tkb.ThuTu == null) tkb.ThuTu = false;
                if (tkb.ThuNam == null) tkb.ThuNam = false;
                if (tkb.ThuSau == null) tkb.ThuSau = false;
                if (tkb.ThuBay == null) tkb.ThuBay = false;

                db.LICHGIANGDAYs.Add(tkb);
                db.SaveChanges();

                foreach (var item in listdate)
                {
                    var DiemDanh = new DIEMDANH();
                    DiemDanh.MaNMH = nHOMMONHOC.MaNMH;
                    DiemDanh.NgayDiemDanh = item;
                    db.DIEMDANHs.Add(DiemDanh);
                    db.SaveChanges();

                    var CTDD_GV = new CTDD_GV();
                    CTDD_GV.MaDD = DiemDanh.MaDD;
                    CTDD_GV.MaGV = (int)nHOMMONHOC.MaGV;
                    CTDD_GV.TTDD = false;
                    db.CTDD_GV.Add(CTDD_GV);
                    db.SaveChanges();
                }




                TimeSpan time = (TimeSpan)nHOMMONHOC.ThoiGianKT - (TimeSpan)nHOMMONHOC.ThoiGianBD;
                var GV = db.GIANGVIENs.Single(s => s.MaGV == nHOMMONHOC.MaGV);
                var PHONG = db.PHONGHOCs.Single(s => s.MaPhongHoc == nHOMMONHOC.MaPhongHoc);
                var myData = new
                {
                    MaNMH = nHOMMONHOC.MaNMH,
                    NgayBD = Convert.ToDateTime(nHOMMONHOC.NgayBD).ToString("dd/MM/yyyy"),
                    NgayKT = Convert.ToDateTime(nHOMMONHOC.NgayKT).ToString("dd/MM/yyyy"),
                    ThoiGianDay = $"{time.Hours} Giờ {time.Minutes} Phút",
                    TenGiangVien = GV.TenGV,
                    TenPhongHoc = PHONG.TenPhongHoc,
                    SoSV = 0
                };
                return Json(myData, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyNhomMonHoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOMMONHOC nHOMMONHOC = db.NHOMMONHOCs.Find(id);
            if (nHOMMONHOC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNMH = new SelectList(db.LICHGIANGDAYs, "MaNMH", "MaNMH", nHOMMONHOC.MaNMH);
            ViewBag.MaMH = new SelectList(db.MONHOCs, "MaMH", "TenMH", nHOMMONHOC.MaMH);
            ViewBag.MaPhongHoc = new SelectList(db.PHONGHOCs, "MaPhongHoc", "TenPhongHoc", nHOMMONHOC.MaPhongHoc);
            return View(nHOMMONHOC);
        }

        // POST: QuanLyNhomMonHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNMH,MaMH,MaPhongHoc,NgayBD,NgayKT,ThoiGianBD,ThoiGianKT")] NHOMMONHOC nHOMMONHOC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHOMMONHOC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNMH = new SelectList(db.LICHGIANGDAYs, "MaNMH", "MaNMH", nHOMMONHOC.MaNMH);
            ViewBag.MaMH = new SelectList(db.MONHOCs, "MaMH", "TenMH", nHOMMONHOC.MaMH);
            ViewBag.MaPhongHoc = new SelectList(db.PHONGHOCs, "MaPhongHoc", "TenPhongHoc", nHOMMONHOC.MaPhongHoc);
            return View(nHOMMONHOC);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOMMONHOC nHOMMONHOC = db.NHOMMONHOCs.Find(id);
            if (nHOMMONHOC == null)
            {
                return HttpNotFound();
            }
            return View(nHOMMONHOC);
        }

        // POST: QuanLyNhomMonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NHOMMONHOC nHOMMONHOC = db.NHOMMONHOCs.Find(id);
            db.NHOMMONHOCs.Remove(nHOMMONHOC);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

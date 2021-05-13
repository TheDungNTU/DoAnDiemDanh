using System;
using System.Collections.Generic;
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

        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();
        // GET: QuanLyMonHoc

        public IEnumerable<DateTime> EachDay(DateTime d1, DateTime d2)
        {
            for (var day = d1.Date; day.Date <= d2.Date; day = day.AddDays(1))
                yield return day;
        }

        public ActionResult DangKyMonHoc()
        {
            ViewBag.MaMH = db.MONHOCs;
            ViewBag.MaSV = db.SINHVIENs;
            return View();
        }

        [HttpPost]
        public JsonResult DangKyMonHoc(string MaMH, string MaSV)
        {
            string sqlconnectStr = "Data Source=DESKTOP-37IIIG3;initial catalog=FACE_RECOGNITION;User ID=sa;Password=dung.lt.59cntt;";
            var cnn = new SqlConnection(sqlconnectStr);
            cnn.Open();

            var command = new SqlCommand($"INSERT INTO SINHVIEN_MONHOC VALUES ({MaSV},{MaMH})", cnn);
            command.ExecuteNonQuery();

            
            int mamh = Int32.Parse(MaMH);
            int masv = Int32.Parse(MaSV);

            var DiemDanh = db.DIEMDANHs.Where(_ => _.MaMH == mamh).ToList();
            var CTDD = new CTDD();
            foreach(var item in DiemDanh)
            {
                command = new SqlCommand($"INSERT INTO dbo.CTDD ( MaDD, MaSV) VALUES  ( {item.MaDD},{masv})", cnn);
                command.ExecuteNonQuery();
            }

            cnn.Close();
            return Json(MaMH, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult Index()
        {
            ViewBag.MaGV = db.GIANGVIENs;
            var mONHOCs = db.MONHOCs.Include(m => m.GIANGVIEN);
            return View(mONHOCs.ToList());
        }

        // GET: QuanLyMonHoc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONHOC mONHOC = db.MONHOCs.Find(id);
            if (mONHOC == null)
            {
                return HttpNotFound();
            }
            return View(mONHOC);
        }

 
        // POST: QuanLyMonHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaMH,TenMH,SoTC,NgayBD,NgayKT,ThoiGianBDGD,ThoiGianKTGD,MaGV")] MONHOC mONHOC, string TenGV)
        {

            if (ModelState.IsValid)
            {
                TimeSpan time = (TimeSpan)mONHOC.ThoiGianKTGD - (TimeSpan)mONHOC.ThoiGianBDGD;
                var listime = EachDay((DateTime)mONHOC.NgayBD, (DateTime)mONHOC.NgayKT);

                var DiemDanh = new DIEMDANH();

                db.MONHOCs.Add(mONHOC);
                db.SaveChanges();

                CultureInfo vn = new CultureInfo("vi-VN");

                foreach (var item in listime)
                {
                    var ngay = vn.DateTimeFormat.GetDayName(item.DayOfWeek);
                    if (ngay != "Chủ Nhật" && ngay != "Thứ Bảy")
                    {
                        DiemDanh.MaMH = mONHOC.MaMH;
                        DiemDanh.NgayDiemDanh = item;
                        db.DIEMDANHs.Add(DiemDanh);
                        db.SaveChanges();
                    }
                }

                var myData = new
                {
                    MaMH = mONHOC.MaMH,
                    TenMH = mONHOC.TenMH,
                    SoTC = mONHOC.SoTC,
                    NgayBD = Convert.ToDateTime(mONHOC.NgayBD).ToString("dd/MM/yyyy"),
                    NgayKT = Convert.ToDateTime(mONHOC.NgayKT).ToString("dd/MM/yyyy"),
                    ThoiGianDay = $"{time.Hours} Giờ {time.Minutes} Phút",
                    GiangVienDay = TenGV,
                    SoSV = 0
                };
                return Json(myData, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyMonHoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONHOC mONHOC = db.MONHOCs.Find(id);
            if (mONHOC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaGV = new SelectList(db.GIANGVIENs, "MaGV", "TenGV", mONHOC.MaGV);
            return View(mONHOC);
        }

        // POST: QuanLyMonHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaMH,TenMH,SoTC,NgayBD,NgayKT,ThoiGianBDGD,ThoiGianKTGD,MaGV")] MONHOC mONHOC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mONHOC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaGV = new SelectList(db.GIANGVIENs, "MaGV", "TenGV", mONHOC.MaGV);
            return View(mONHOC);
        }

        // GET: QuanLyMonHoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONHOC mONHOC = db.MONHOCs.Find(id);
            if (mONHOC == null)
            {
                return HttpNotFound();
            }
            return View(mONHOC);
        }

        // POST: QuanLyMonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            MONHOC mONHOC = db.MONHOCs.Find(id);
            db.MONHOCs.Remove(mONHOC);
            db.SaveChanges();
            return Json(id,JsonRequestBehavior.AllowGet);
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

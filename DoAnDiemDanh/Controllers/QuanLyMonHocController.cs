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
        
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();
        // GET: QuanLyMonHoc

        [Authorize(Roles = "Admin, GiangVien")]
        public IEnumerable<DateTime> EachDay(DateTime d1, DateTime d2)
        {
            for (var day = d1.Date; day.Date <= d2.Date; day = day.AddDays(1))
                yield return day;
        }

        [Authorize(Roles = "Admin, GiangVien")]
        public ActionResult DangKyMonHoc()
        {
            ViewBag.MaMH = db.MONHOCs;
            ViewBag.Khoa_Filter = db.KHOAs;
            ViewBag.Lop_Filter = db.LOPs;
            return View();
        }

        [Authorize(Roles = "Admin, GiangVien")]
        [HttpGet]
        public JsonResult GetThongTinMonHoc(int MaMH)
        {
            
            var MonHoc = db.MONHOCs.SingleOrDefault(_ => _.MaMH == MaMH);

            var SinhVienDK = from sv in db.SINHVIENs
                             where sv.MONHOCs.Any(mh => mh.MaMH == MaMH)
                             select sv.MaSV;

            var SinhVien = from sv in db.SINHVIENs
                           where !SinhVienDK.Contains(sv.MaSV)
                           select new { MaSV = sv.MaSV, TenSV = sv.TenSV, TenKhoa = sv.KHOA.TenKhoa, TenLop = sv.LOP.TenLop };
            JsonResult categoryJson = new JsonResult();
            categoryJson.Data = SinhVien;
            var data = new
            {
                TenMH = MonHoc.TenMH,
                TenGV = MonHoc.GIANGVIEN.TenGV,
                SoTC = MonHoc.SoTC,
                SinhVien = categoryJson,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, GiangVien")]
        [HttpPost]
        public JsonResult DangKyMonHoc(string MaMH, List<string> data)
        {
            string sqlconnectStr = ConfigurationManager.ConnectionStrings["ASPNETConnectionString"].ToString();
            var cnn = new SqlConnection(sqlconnectStr);
            cnn.Open();
            foreach(var item in data)
            {
                var command = new SqlCommand($"INSERT INTO SINHVIEN_MONHOC VALUES ({item},{MaMH})", cnn);
                command.ExecuteNonQuery();

                int mamh = Int32.Parse(MaMH);
                int masv = Int32.Parse(item);

                var DiemDanh = db.DIEMDANHs.Where(_ => _.MaMH == mamh).ToList();
                var CTDD = new CTDD();
                foreach (var item1 in DiemDanh)
                {
                    command = new SqlCommand($"INSERT INTO dbo.CTDD ( MaDD, MaSV) VALUES  ( {item1.MaDD},{masv})", cnn);
                    command.ExecuteNonQuery();
                }
            }
           
            cnn.Close();
            return Json(MaMH, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.MaGV = db.GIANGVIENs;
            var mONHOCs = db.MONHOCs.Include(m => m.GIANGVIEN);
            return View(mONHOCs.ToList());
        }
        [Authorize(Roles = "Admin")]
        // POST: QuanLyMonHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaMH,TenMH,SoTC,NgayBD,NgayKT,ThoiGianBDGD,ThoiGianKTGD,MaGV")] MONHOC mONHOC, string TenGV, List<int> TKB)
        {
            
            if (ModelState.IsValid)
            {
                TimeSpan time = (TimeSpan)mONHOC.ThoiGianKTGD - (TimeSpan)mONHOC.ThoiGianBDGD;
                var listime = EachDay((DateTime)mONHOC.NgayBD, (DateTime)mONHOC.NgayKT);

                db.MONHOCs.Add(mONHOC);
                db.SaveChanges();


                THOIKHOABIEU tkb = new THOIKHOABIEU();
                tkb.MaMH = mONHOC.MaMH;

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

                db.THOIKHOABIEUx.Add(tkb);
                db.SaveChanges();

             

                foreach (var item in listime)
                {
                    var ngay = (int)item.DayOfWeek;
                    var flag = TKB.Contains(ngay);
                    
                    if (flag)
                    {
                        var DiemDanh = new DIEMDANH();
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        // POST: QuanLyMonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            MONHOC mONHOC = db.MONHOCs.Find(id);
            var data = new
            {
                id = id,
                TenMH = mONHOC.TenMH
            };
            db.MONHOCs.Remove(mONHOC);
            db.SaveChanges();
            return Json(data,JsonRequestBehavior.AllowGet);
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

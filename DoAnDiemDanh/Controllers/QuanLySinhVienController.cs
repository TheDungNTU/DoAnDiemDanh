using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.OleDb;
using Newtonsoft.Json;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class QuanLySinhVienController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        [HttpPost]
        public JsonResult ImportFileExcel(HttpPostedFileBase excel)
        {
            string filename = excel.FileName;
            string targetpath = Server.MapPath("~/Content/doc/");
            excel.SaveAs(targetpath + filename);
            string pathToExcelFile = targetpath + filename;
            var connectionString = "";
            if (filename.EndsWith(".xls"))
            {
                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
            }
            else if (filename.EndsWith(".xlsx"))
            {
                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
            }

            var conn = new OleDbConnection(connectionString);
            if (conn.State == ConnectionState.Closed) conn.Open();
            string SpreadSheetName = "";
            DataTable ExcelSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

            SpreadSheetName = ExcelSheets.Rows[0]["TABLE_NAME"].ToString();
            var query = "SELECT * FROM [" + SpreadSheetName + "]";
            var cmd = new OleDbCommand(query, conn);
            var data = new OleDbDataAdapter(cmd);
            var dt = new DataTable();
            data.Fill(dt);

            List<SinhVienRows> listSV = new List<SinhVienRows>();


            foreach (DataRow item in dt.Rows)
            {
                if(item["STT"].ToString() != "")
                {
                    var tenkhoa = item["Tên Khoa"].ToString();
                    var tenlop = item["Tên Lớp"].ToString();
                    KHOA Khoa = db.KHOAs.Single(_ => _.TenKhoa.Contains(tenkhoa));
                    LOP lop = db.LOPs.Single(_ => _.TenLop.Contains(tenlop));
                    if(Khoa != null && lop != null)
                    {
                        SINHVIEN sv = new SINHVIEN();
                        SinhVienRows svr = new SinhVienRows();
                        sv.TenSV = item["Họ Và Tên"].ToString();
                        svr.TenSV = sv.TenSV;
                        svr.TenKhoa = Khoa.TenKhoa;
                        svr.TenLop = lop.TenLop;
                        sv.MaKhoa = Khoa.MaKhoa;
                        sv.MaLop = lop.MaLop;
                        db.SINHVIENs.Add(sv);
                        db.SaveChanges();
                        svr.MaSV = sv.MaSV;
                        listSV.Add(svr);
                    }
                }
            }
            conn.Close();
            return Json(listSV, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLySinhVien
        public ActionResult Index()
        {
            var sINHVIENs = db.SINHVIENs.Include(sv => sv.KHOA).Include(sv => sv.LOP);
            ViewBag.Khoa = db.KHOAs.Where(_ => _.LOPs.Count() > 0);
            ViewBag.Khoa_Filter = db.KHOAs;
            ViewBag.Lop_Filter = db.LOPs;
            return View(sINHVIENs.ToList());
        }

        public JsonResult GetLopHoc(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var LOPHOC = db.LOPs.Where(_ => _.MaKhoa == id).ToList();
            return Json(LOPHOC, JsonRequestBehavior.AllowGet);
        }



        // POST: QuanLySinhVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaSV,TenSV,MaKhoa,MaLop")] SINHVIEN sINHVIEN)
        {
            var Khoa = db.KHOAs.Where(_ => _.MaKhoa == sINHVIEN.MaKhoa).Single();
            var Lop = db.LOPs.Where(_ => _.MaLop == sINHVIEN.MaLop).Single();
            db.SINHVIENs.Add(sINHVIEN);
                db.SaveChanges();
            var data = new
            {
                MaSV = sINHVIEN.MaSV,
                TenSV = sINHVIEN.TenSV,
                TenKhoa = Khoa.TenKhoa,
                TenLop = Lop.TenLop,
            };
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLySinhVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SINHVIEN sINHVIEN = db.SINHVIENs.Find(id);
            if (sINHVIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", sINHVIEN.MaKhoa);
            ViewBag.MaLop = new SelectList(db.LOPs, "MaLop", "TenLop", sINHVIEN.MaLop);
            return View(sINHVIEN);
        }

        // POST: QuanLySinhVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSV,TenSV,MaKhoa,MaLop")] SINHVIEN sINHVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sINHVIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", sINHVIEN.MaKhoa);
            ViewBag.MaLop = new SelectList(db.LOPs, "MaLop", "TenLop", sINHVIEN.MaLop);
            return View(sINHVIEN);
        }

        // GET: QuanLySinhVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SINHVIEN sINHVIEN = db.SINHVIENs.Find(id);
            if (sINHVIEN == null)
            {
                return HttpNotFound();
            }
            return View(sINHVIEN);
        }

        // POST: QuanLySinhVien/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            SINHVIEN sINHVIEN = db.SINHVIENs.Find(id);
            var data = new
            {
                id = id,
                TenSV = sINHVIEN.TenSV,
            };
            db.SINHVIENs.Remove(sINHVIEN);
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private class SinhVienRows
        {
            public int MaSV { get; set; }
            public string TenSV { get; set; }

            public string TenKhoa { get; set; }

            public string TenLop { get; set; }

        }
    }
}

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
    [Authorize(Roles = "Admin")]
    public class QuanLyGiangVienController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        // GET: QuanLyGiangVien
        public ActionResult Index()
        {
            var gIANGVIENs = db.GIANGVIENs.Include(g => g.KHOA);
            ViewBag.MONHOC = db.MONHOCs.Include(_ => _.GIANGVIEN);
            ViewBag.MaKhoa = db.KHOAs;
            return View(gIANGVIENs.ToList());
        }

         [HttpPost]
         public JsonResult Create([Bind(Include = "MaGV,TenGV,Email,MaKhoa")] GIANGVIEN gIANGVIEN)
         {
    
            var Khoa = db.KHOAs.Where(_ => _.MaKhoa == gIANGVIEN.MaKhoa).Single();

            var check = db.GIANGVIENs.Any(_ => _.Email == gIANGVIEN.Email);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            db.GIANGVIENs.Add(gIANGVIEN);
            db.SaveChanges();

            TAIKHOAN tk = new TAIKHOAN();
            tk.MaQuyen = 2;
            tk.MatKhau = "user123";
            tk.MaGV = gIANGVIEN.MaGV;
            tk.TaiKhoan1 = gIANGVIEN.Email;

            db.TAIKHOANs.Add(tk);
            db.SaveChanges();

            var data = new
            {
                MaGV = gIANGVIEN.MaGV,
                TenGV = gIANGVIEN.TenGV,
                Email = gIANGVIEN.Email,
                TenKhoa = Khoa.TenKhoa

            };
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyGiangVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);
            if (gIANGVIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
            return View(gIANGVIEN);
        }

        // POST: QuanLyGiangVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaGV,TenGV,Password,MaKhoa")] GIANGVIEN gIANGVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gIANGVIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
            return View(gIANGVIEN);
        }

        // POST: QuanLyGiangVien/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);
            db.GIANGVIENs.Remove(gIANGVIEN);
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

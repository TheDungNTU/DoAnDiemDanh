using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuanLyGiangVienController : Controller
    {
        private FACE_RECOGNITION_V2Entities db = new FACE_RECOGNITION_V2Entities();

        public void SendMail(string email, string subject, string content)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("tuyetsuong6332@gmail.com");
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = content;
                mail.IsBodyHtml = true;


                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("tuyetsuong6332@gmail.com", "0987806758");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

        }


        //GET: QuanLyGiangVien
        public ActionResult Index()
        {
            var gIANGVIENs = db.GIANGVIENs.Include(g => g.KHOA);
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

            TAIKHOANGIANGVIEN tk = new TAIKHOANGIANGVIEN();
            tk.MaQuyen = 2;
            tk.MatKhau = "user123";
            tk.MaGV = gIANGVIEN.MaGV;
            tk.TaiKhoan = gIANGVIEN.Email;

            db.TAIKHOANGIANGVIENs.Add(tk);
            db.SaveChanges();

            SendMail(gIANGVIEN.Email, "[MẬT KHẨU ĐĂNG NHẬP WEB ĐIỂM DANH]", "Mật khẩu của bạn là: user123");

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
        public ActionResult Edit([Bind(Include = "MaGV,TenGV,Email,MaKhoa")] GIANGVIEN gIANGVIEN)
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
        //[HttpPost, ActionName("Delete")]
        //public JsonResult DeleteConfirmed(int id)
        //{
        //    GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);

        //    var data = new
        //    {
        //        id = id,
        //        TenGV = gIANGVIEN.TenGV,
        //    };
        //    var TaiKhoan = db.TAIKHOANGIANGVIENs.SingleOrDefault(s => s.MaGV == id);
        //    var MonHoc = db.MONHOCs.SingleOrDefault(s => s.MaGV == id);
        //    if(MonHoc == null && TaiKhoan.MaQuyen != 1)
        //    {
        //        db.TAIKHOANGIANGVIENs.Remove(TaiKhoan);
        //        db.SaveChanges();
        //    }
        //    db.GIANGVIENs.Remove(gIANGVIEN);
        //    db.SaveChanges();

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

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

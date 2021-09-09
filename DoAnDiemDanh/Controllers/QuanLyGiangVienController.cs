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
        private BaseModel db = new BaseModel();

        public void SendMail(string email, string Subject, string content)
        {
            var fromEmail = new MailAddress("noreply@vinaai.com", "VinaAI_Test");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "m%CS99SGvina"; // Replace with actual password
            string subject = Subject;

            string body = content;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


        //GET: QuanLyGiangVien
        public ActionResult Index()
        {
            var gIANGVIENs = db.Entity.GIANGVIENs.Include(g => g.KHOA);
            ViewBag.MaKhoa = db.Entity.KHOAs;
            if (db.Entity.PHONGHOCs.Count() > 0)
            {
                ViewBag.MaPhongHoc = db.Entity.PHONGHOCs.First().MaPhongHoc;
            }
           
            return View(gIANGVIENs.ToList());
        }

         [HttpPost]
         public JsonResult Create([Bind(Include = "MaGV,TenGV,Email,MaKhoa")] GIANGVIEN gIANGVIEN)
         {
    
            var Khoa = db.Entity.KHOAs.Where(_ => _.MaKhoa == gIANGVIEN.MaKhoa).Single();

            var check = db.Entity.GIANGVIENs.Any(_ => _.Email == gIANGVIEN.Email);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            db.Entity.GIANGVIENs.Add(gIANGVIEN);
            db.Entity.SaveChanges();

            TAIKHOANGIANGVIEN tk = new TAIKHOANGIANGVIEN();
            tk.MaQuyen = 2;
            tk.MatKhau = "user123";
            tk.MaGV = gIANGVIEN.MaGV;
            tk.TaiKhoan = gIANGVIEN.Email;

            db.Entity.TAIKHOANGIANGVIENs.Add(tk);
            db.Entity.SaveChanges();

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
            GIANGVIEN gIANGVIEN = db.Entity.GIANGVIENs.Find(id);
            if (gIANGVIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.Entity.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
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
                db.Entity.Entry(gIANGVIEN).State = EntityState.Modified;
                db.Entity.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.Entity.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
            return View(gIANGVIEN);
        }

        // POST: QuanLyGiangVien/Delete/5
        //[HttpPost, ActionName("Delete")]
        //public JsonResult DeleteConfirmed(int id)
        //{
        //    GIANGVIEN gIANGVIEN = db.Entity.GIANGVIENs.Find(id);

        //    var data = new
        //    {
        //        id = id,
        //        TenGV = gIANGVIEN.TenGV,
        //    };
        //    var TaiKhoan = db.Entity.TAIKHOANGIANGVIENs.SingleOrDefault(s => s.MaGV == id);
        //    var MonHoc = db.Entity.MONHOCs.SingleOrDefault(s => s.MaGV == id);
        //    if(MonHoc == null && TaiKhoan.MaQuyen != 1)
        //    {
        //        db.Entity.TAIKHOANGIANGVIENs.Remove(TaiKhoan);
        //        db.Entity.SaveChanges();
        //    }
        //    db.Entity.GIANGVIENs.Remove(gIANGVIEN);
        //    db.Entity.SaveChanges();

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

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

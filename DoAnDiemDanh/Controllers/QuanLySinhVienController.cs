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
using ExcelDataReader;
using System.Net.Mail;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin, GiangVien")]
    public class QuanLySinhVienController : Controller
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

        [HttpPost]
        public JsonResult ImportFileExcel(HttpPostedFileBase excel)
        {
            string filename = excel.FileName;
            string targetpath = Server.MapPath("~/Content/doc/");
            excel.SaveAs(targetpath + filename);
            string pathToExcelFile = targetpath + filename;

            List<SinhVienRows> listSV = new List<SinhVienRows>();
            using (var stream = System.IO.File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // Use first row is ColumnName here :D
                    }
                });
                DataTable dt = result.Tables[0];

                foreach (DataRow item in dt.Rows)
                {
                    var email = item["Email"].ToString();
                    if (db.Entity.SINHVIENs.Any(_ => _.Email == email))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }

                foreach (DataRow item in dt.Rows)
                {
                    if (item["STT"].ToString() != "")
                    {
                        var tenkhoa = item["Tên Khoa"].ToString();
                        var tenlop = item["Tên Lớp"].ToString();
                        var diachi = item["Địa chỉ"].ToString();
                        var sdt = item["SĐT"].ToString();
                        var email = item["Email"].ToString();
                        KHOA Khoa = db.Entity.KHOAs.Single(_ => _.TenKhoa.Contains(tenkhoa));
                        LOP lop = db.Entity.LOPs.Single(_ => _.TenLop.Contains(tenlop));
                        if (Khoa != null && lop != null)
                        {
                            SINHVIEN sv = new SINHVIEN();
                            SinhVienRows svr = new SinhVienRows();
                            sv.TenSV = item["Họ Và Tên"].ToString();
                            svr.TenSV = sv.TenSV;
                            svr.TenKhoa = Khoa.TenKhoa;
                            svr.TenLop = lop.TenLop;
                            sv.MaKhoa = Khoa.MaKhoa;
                            sv.MaLop = lop.MaLop;
                            sv.DiaChi = diachi;
                            svr.DiaChi = diachi;
                            sv.SDT = sdt;
                            svr.SDT = sdt;
                            sv.Email = email;
                            svr.Email = email;
                            db.Entity.SINHVIENs.Add(sv);
                            db.Entity.SaveChanges();

                            var tk = new TAIKHOANSINHVIEN();
                            tk.MaQuyen = 3;
                            tk.MatKhau = "sinhvien123";
                            tk.MaSV = sv.MaSV;
                            tk.TaiKhoan = sv.Email;
                            db.Entity.TAIKHOANSINHVIENs.Add(tk);
                            db.Entity.SaveChanges();

                           

                            svr.MaSV = sv.MaSV;
                            listSV.Add(svr);
                        }
                    }
                }
                stream.Close();
            }
            return Json(listSV, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLySinhVien
        public ActionResult Index()
        {
          
            var sINHVIENs = db.Entity.SINHVIENs.Include(sv => sv.KHOA).Include(sv => sv.LOP);
            
            ViewBag.Khoa = db.Entity.KHOAs.Where(_ => _.LOPs.Count() > 0);
            ViewBag.Khoa_Filter = db.Entity.KHOAs;
            ViewBag.Lop_Filter = db.Entity.LOPs;
            if (db.Entity.PHONGHOCs.Count() > 0)
            {
                ViewBag.MaPhongHoc = db.Entity.PHONGHOCs.First().MaPhongHoc;
            }     
            return View(sINHVIENs.ToList());
        }

        public JsonResult GetLopHoc(int id)
        {
            db.Entity.Configuration.ProxyCreationEnabled = false;
            var LOPHOC = db.Entity.LOPs.Where(_ => _.MaKhoa == id).ToList();
            return Json(LOPHOC, JsonRequestBehavior.AllowGet);
        }



        // POST: QuanLySinhVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaSV,TenSV,Email,DiaChi,SDT,MaKhoa,MaLop")] SINHVIEN sINHVIEN)
        {
            var Khoa = db.Entity.KHOAs.Where(_ => _.MaKhoa == sINHVIEN.MaKhoa).Single();
            var Lop = db.Entity.LOPs.Where(_ => _.MaLop == sINHVIEN.MaLop).Single();

            var check1 = db.Entity.GIANGVIENs.Any(_ => _.Email == sINHVIEN.Email);
            var check2 = db.Entity.SINHVIENs.Any(_ => _.Email == sINHVIEN.Email);
            if (check1 || check2)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            db.Entity.SINHVIENs.Add(sINHVIEN);
            db.Entity.SaveChanges();

            TAIKHOANSINHVIEN tk = new TAIKHOANSINHVIEN();
            tk.MaQuyen = 3;
            tk.MatKhau = "sinhvien123";
            tk.MaSV = sINHVIEN.MaSV;
            tk.TaiKhoan =sINHVIEN.Email;

            db.Entity.TAIKHOANSINHVIENs.Add(tk);
            db.Entity.SaveChanges();
            SendMail(sINHVIEN.Email, "[MẬT KHẨU ĐĂNG NHẬP WEB ĐIỂM DANH]", "Mật khẩu của bạn là: sinhvien123");
            var data = new
            {
                MaSV = sINHVIEN.MaSV,
                TenSV = sINHVIEN.TenSV,
                Email = sINHVIEN.Email,
                SDT = sINHVIEN.SDT,
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
            SINHVIEN sINHVIEN = db.Entity.SINHVIENs.Find(id);
            if (sINHVIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.Entity.KHOAs, "MaKhoa", "TenKhoa", sINHVIEN.MaKhoa);
            ViewBag.MaLop = new SelectList(db.Entity.LOPs, "MaLop", "TenLop", sINHVIEN.MaLop);
            return View(sINHVIEN);
        }

        // POST: QuanLySinhVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSV,TenSV,MaKhoa,MaLop,Avatar,Email,DiaChi,SDT")] SINHVIEN sINHVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entity.Entry(sINHVIEN).State = EntityState.Modified;
                db.Entity.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.Entity.KHOAs, "MaKhoa", "TenKhoa", sINHVIEN.MaKhoa);
            ViewBag.MaLop = new SelectList(db.Entity.LOPs, "MaLop", "TenLop", sINHVIEN.MaLop);
            return View(sINHVIEN);
        }

        // GET: QuanLySinhVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SINHVIEN sINHVIEN = db.Entity.SINHVIENs.Find(id);
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
            SINHVIEN sINHVIEN = db.Entity.SINHVIENs.Find(id);
            TAIKHOANSINHVIEN tk = db.Entity.TAIKHOANSINHVIENs.Single(_ => _.MaSV == sINHVIEN.MaSV);
            db.Entity.TAIKHOANSINHVIENs.Remove(tk);
            db.Entity.SaveChanges();
            var data = new
            {
                id = id,
                TenSV = sINHVIEN.TenSV,
            };
            db.Entity.SINHVIENs.Remove(sINHVIEN);
            db.Entity.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Entity.Dispose();
            }
            base.Dispose(disposing);
        }

        private class SinhVienRows
        {
            public int MaSV { get; set; }
            public string TenSV { get; set; }
            public string TenKhoa { get; set; }
            public string DiaChi { get; set; }
            public string SDT { get; set; }
            public string Email { get; set; }
            public string TenLop { get; set; }

        }
    }
}

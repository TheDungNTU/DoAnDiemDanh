using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuanLyHinhAnhController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        public static string RandomString(int length)
        {
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

        [HttpPost]
        public JsonResult Webcam(int id, object data)
        {
 
            var  list = (IEnumerable<string>) data;
            foreach(var item in list)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item)))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        var postedFileName = $"{id}_{RandomString(10)}.jpg";
                        var path = Server.MapPath("/Content/img/" + postedFileName);
                        bm2.Save(path);

                        HINHANH hinhanh = new HINHANH();
                        hinhanh.MaSV = id;
                        hinhanh.TenHA = postedFileName;
                        hinhanh.BASE64 = item;
                        db.HINHANHs.Add(hinhanh);
                        db.SaveChanges();
                    }
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        //Hiển thị thông tin hình ảnh của sinh viên
        public ActionResult Details_SinhVien_HinhAnh(int? id)
        {
            var HinhAnhs = db.HINHANHs.Where(s => s.MaSV == id);
            ViewBag.TenSV = db.SINHVIENs.SingleOrDefault(s => s.MaSV == id).TenSV;
            ViewBag.MaSV = id;
            return View(HinhAnhs.ToList());
        }

        [HttpPost]
        public JsonResult Details_SinhVien_HinhAnh(int id)
        {

            var img = Request.Files["Avatar"];

            string postedFileName = System.IO.Path.GetFileName(img.FileName);

            //Lưu hình đại diện về Server
            var path = Server.MapPath("/Content/img/" + postedFileName);
            img.SaveAs(path);

            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var HinhAnh = new HINHANH();
            HinhAnh.MaSV = id;
            HinhAnh.TenHA = postedFileName;
            HinhAnh.BASE64 = base64ImageRepresentation;

            db.HINHANHs.Add(HinhAnh);
            db.SaveChanges();
            return Json(HinhAnh, JsonRequestBehavior.AllowGet);
      
       }

        // GET: QuanLyHinhAnh
        public ActionResult Index()
        {
            var hINHANHs = db.HINHANHs.Include(h => h.SINHVIEN);
            return View(hINHANHs.ToList());
        }

        // GET: QuanLyHinhAnh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HINHANH hINHANH = db.HINHANHs.Find(id);
            if (hINHANH == null)
            {
                return HttpNotFound();
            }
            return View(hINHANH);
        }

        // GET: QuanLyHinhAnh/Create
        public ActionResult Create()
        {
            ViewBag.MaSV = new SelectList(db.SINHVIENs, "MaSV", "TenSV");
            return View();
        }

        // POST: QuanLyHinhAnh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHA,TenHA,MaSV,BASE64")] HINHANH hINHANH)
        {
            if (ModelState.IsValid)
            {
                db.HINHANHs.Add(hINHANH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaSV = new SelectList(db.SINHVIENs, "MaSV", "TenSV", hINHANH.MaSV);
            return View(hINHANH);
        }

        // GET: QuanLyHinhAnh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HINHANH hINHANH = db.HINHANHs.Find(id);
            if (hINHANH == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaSV = new SelectList(db.SINHVIENs, "MaSV", "TenSV", hINHANH.MaSV);
            return View(hINHANH);
        }

 
        // POST: QuanLyHinhAnh/Delete/5
        [HttpPost, ActionName("Delete")]

        public JsonResult DeleteConfirmed(int id)
        {
            HINHANH hINHANH = db.HINHANHs.Find(id);
            db.HINHANHs.Remove(hINHANH);
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

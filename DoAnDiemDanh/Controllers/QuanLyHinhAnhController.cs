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
    [Authorize(Roles = "Admin, GiangVien")]
    public class QuanLyHinhAnhController : Controller
    {
        private BaseModel db = new BaseModel();

       
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
            List<HinhCam> listAnh = new List<HinhCam>();
            foreach(var item in list)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item)))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        var postedFileName = $"SV_{id}_{RandomString(10)}.jpg";
                        var path = Server.MapPath("/Content/img/" + postedFileName);
                        bm2.Save(path);

                        HINHANH_SV hinhanh = new HINHANH_SV();
                        hinhanh.MaSV = id;
                        hinhanh.TenHA = postedFileName;
                        hinhanh.BASE64 = item;
                        db.Entity.HINHANH_SV.Add(hinhanh);
                        db.Entity.SaveChanges();

                        var hinhcam = new HinhCam();
                        hinhcam.tenanh = postedFileName;
                        hinhcam.maanh = hinhanh.MaHA;
                        listAnh.Add(hinhcam);
                    }
                }
            }
            return Json(listAnh, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Webcam_GV(int id, object data)
        {

            var list = (IEnumerable<string>)data;
            List<HinhCam> listAnh = new List<HinhCam>();
            foreach (var item in list)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item)))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        var postedFileName = $"GV_{id}_{RandomString(10)}.jpg";
                        var path = Server.MapPath("/Content/img/" + postedFileName);
                        bm2.Save(path);

                        HINHANH_GV hinhanh = new HINHANH_GV();
                        hinhanh.MaGV = id;
                        hinhanh.TenHA = postedFileName;
                        hinhanh.BASE64 = item;
                        db.Entity.HINHANH_GV.Add(hinhanh);
                        db.Entity.SaveChanges();

                        var hinhcam = new HinhCam();
                        hinhcam.tenanh = postedFileName;
                        hinhcam.maanh = hinhanh.MaHA;
                        listAnh.Add(hinhcam);
                    }
                }
            }
            return Json(listAnh, JsonRequestBehavior.AllowGet);
        }

        //Hiển thị thông tin hình ảnh của sinh viên
        public ActionResult Details_SinhVien_HinhAnh(int? id, int MaPhongHoc)
        {
            var HinhAnhs = db.Entity.HINHANH_SV.Where(s => s.MaSV == id);
            var PHONGHOC = db.Entity.PHONGHOCs.Single(s => s.MaPhongHoc == MaPhongHoc);
            ViewBag.PHONGHOC = db.Entity.PHONGHOCs.Where(s => s.MaPhongHoc != MaPhongHoc).ToList();
            ViewBag.TenSV = db.Entity.SINHVIENs.SingleOrDefault(s => s.MaSV == id).TenSV;
            ViewBag.MaSV = id;
            ViewBag.Url = PHONGHOC.CAMERA.URL;
            ViewBag.MaPhongHoc = MaPhongHoc;
            ViewBag.TenPhongHoc = PHONGHOC.TenPhongHoc;
            return View(HinhAnhs.ToList());
        }

        public ActionResult Details_GiangVien_HinhAnh(int? id, int MaPhongHoc)
        {
            var PHONGHOC = db.Entity.PHONGHOCs.Single(s => s.MaPhongHoc == MaPhongHoc);
            var HinhAnhs = db.Entity.HINHANH_GV.Where(s => s.MaGV == id);
            ViewBag.PHONGHOC = db.Entity.PHONGHOCs.Where(s => s.MaPhongHoc != MaPhongHoc).ToList();
            ViewBag.TenGV = db.Entity.GIANGVIENs.SingleOrDefault(s => s.MaGV == id).TenGV;
            ViewBag.MaGV = id;
            ViewBag.Url = PHONGHOC.CAMERA.URL;
            ViewBag.MaPhongHoc = MaPhongHoc;
            ViewBag.TenPhongHoc = PHONGHOC.TenPhongHoc;
            return View(HinhAnhs.ToList());
        }

        [HttpPost]
        public JsonResult Details_SinhVien_HinhAnh(int id)
        {

            var img = Request.Files["Avatar"];

            string postedFileName = "SV_"+ id + "_" + System.IO.Path.GetFileName(img.FileName);

            //Lưu hình đại diện về Server
            var path = Server.MapPath("/Content/img/" + ""+postedFileName);
            img.SaveAs(path);

            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var HinhAnh = new HINHANH_SV();
            HinhAnh.MaSV = id;
            HinhAnh.TenHA = postedFileName;
            HinhAnh.BASE64 = base64ImageRepresentation;

            db.Entity.HINHANH_SV.Add(HinhAnh);
            db.Entity.SaveChanges();
            return Json(HinhAnh, JsonRequestBehavior.AllowGet);
      
       }

        [HttpPost]
        public JsonResult Details_GiangVien_HinhAnh(int id)
        {

            var img = Request.Files["Avatar"];

            string postedFileName = "GV_" + id + "_" + System.IO.Path.GetFileName(img.FileName);

            //Lưu hình đại diện về Server
            var path = Server.MapPath("/Content/img/" + "" + postedFileName);
            img.SaveAs(path);

            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var HinhAnh = new HINHANH_GV();
            HinhAnh.MaGV = id;
            HinhAnh.TenHA = postedFileName;
            HinhAnh.BASE64 = base64ImageRepresentation;

            db.Entity.HINHANH_GV.Add(HinhAnh);
            db.Entity.SaveChanges();
            return Json(HinhAnh, JsonRequestBehavior.AllowGet);
        }
 
        // POST: QuanLyHinhAnh/Delete/5
        [HttpPost, ActionName("Delete")]

        public JsonResult DeleteConfirmed(int id)
        {
            HINHANH_SV hINHANH = db.Entity.HINHANH_SV.Find(id);
            db.Entity.HINHANH_SV.Remove(hINHANH);
            db.Entity.SaveChanges();
            return Json(id,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete_HinhAnh_GV(int id)
        {
            HINHANH_GV hINHANH = db.Entity.HINHANH_GV.Find(id);
            db.Entity.HINHANH_GV.Remove(hINHANH);
            db.Entity.SaveChanges();
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Entity.Dispose();
            }
            base.Dispose(disposing);
        }

        class HinhCam
        {
            public string tenanh { get; set; }
            public int maanh { get; set; }
        }
    }
}

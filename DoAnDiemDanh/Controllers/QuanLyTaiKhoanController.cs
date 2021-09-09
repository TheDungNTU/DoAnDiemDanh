using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    public class QuanLyTaiKhoanController : Controller
    {
        private BaseModel db = new BaseModel();

        public string getusermail()
        {
            HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                if (!string.IsNullOrEmpty(authCookie.Value))
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    string str = authTicket.UserData;
                    string[] subs = str.Split(',');
                    //return Int32.Parse(subs[1]);
                    return subs[1];
                }
            }
            return "";
        }

        [HttpPost]
        public JsonResult CheckMatKhau(string MatKhau)
        {

            var user = getusermail();
            var emailSV = db.Entity.SINHVIENs.Where(x => x.TenSV == user).SingleOrDefault()?.Email;
            var emailGV = db.Entity.GIANGVIENs.Where(x => x.TenGV == user).SingleOrDefault()?.Email;

            var MaSV = db.Entity.TAIKHOANSINHVIENs.Where(x => x.TaiKhoan == emailSV).SingleOrDefault()?.MaSV;

            var MaGV = db.Entity.TAIKHOANGIANGVIENs.Where(x => x.TaiKhoan == emailGV).SingleOrDefault()?.MaGV;
            MatKhau = Crypto.Hash(MatKhau);
            var SinhVien = db.Entity.TAIKHOANSINHVIENs.Where(_ => _.MaSV == MaSV);
            var GiangVien = db.Entity.TAIKHOANGIANGVIENs.Where(_ => _.MaGV == MaGV);

            if (SinhVien.Count() > 0)
            {
                var flag = (SinhVien.First().MatKhau == MatKhau);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else if (GiangVien.Count() > 0)
            {
                var flag = (GiangVien.First().MatKhau == MatKhau);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DoiMatKhau(string MatKhauMoi)
        {
            var user = getusermail();
            var emailSV = db.Entity.SINHVIENs.Where(x => x.TenSV == user).SingleOrDefault()?.Email;
            var emailGV = db.Entity.GIANGVIENs.Where(x => x.TenGV == user).SingleOrDefault()?.Email;

            var MaGV = db.Entity.TAIKHOANGIANGVIENs.Where(x => x.TaiKhoan == emailGV).SingleOrDefault()?.MaGV;
            var MaSV = db.Entity.TAIKHOANSINHVIENs.Where(x => x.TaiKhoan == emailSV).SingleOrDefault()?.MaSV;



            if (MaGV != null)
            {
                TAIKHOANGIANGVIEN nUser = db.Entity.TAIKHOANGIANGVIENs.Single(u => u.MaGV == MaGV);
                nUser.MatKhau = Crypto.Hash(MatKhauMoi);
                db.Entity.SaveChanges();

                var cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLMainDatabase"].ToString());
                cnn.Open();
                string sql = "UPDATE dbo.[User] SET user_pass = '" + Crypto.Hash(MatKhauMoi) + "' WHERE user_name='" + user + "'";

                var command = new SqlCommand(sql, cnn);
                command.ExecuteNonQuery();

                cnn.Close();

                return Json("true", JsonRequestBehavior.AllowGet);
            }
            else if (MaSV != null)
            {
                TAIKHOANSINHVIEN nUser = db.Entity.TAIKHOANSINHVIENs.Single(u => u.MaSV == MaSV);
                nUser.MatKhau = Crypto.Hash(MatKhauMoi);
                db.Entity.SaveChanges();



                return Json("true", JsonRequestBehavior.AllowGet);
            }
            return Json("false", JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public JsonResult DoiAvatar(string HoTen, string DiaChi, string SDT)
        {

            string hoten = HoTen;
            string diachi = DiaChi;
            string sdt = SDT;

            var user = getusermail();
            // var emailSV = db.Entity.SINHVIENs.Where(x => x.TenSV == user).SingleOrDefault()?.Email;
            var emailGV = db.Entity.GIANGVIENs.Where(x => x.TenGV == user).SingleOrDefault()?.Email;

            var MaGV = db.Entity.TAIKHOANGIANGVIENs.Where(x => x.TaiKhoan == emailGV).SingleOrDefault()?.MaGV;
            // var MaSV = db.Entity.TAIKHOANSINHVIENs.Where(x => x.TaiKhoan == emailSV).SingleOrDefault()?.MaSV;

            var GiangVien = db.Entity.GIANGVIENs.Single(_ => _.MaGV == MaGV);

            var img = Request.Files["Avatar1"];

            if (img != null)
            {
                string postedFileName =$"{MaGV}_{System.IO.Path.GetFileName(img.FileName)}";


                //Lưu hình đại diện về Server
                var path = Server.MapPath("/Content/img/avatar/" + postedFileName);
                img.SaveAs(path);
                GiangVien.Avatar = postedFileName;
                GiangVien.HoTen = hoten;
                GiangVien.DiaChi = diachi;
                GiangVien.SDT = sdt;
            }

            db.Entity.Entry(GiangVien).State = EntityState.Modified;
            db.Entity.SaveChanges();

            // Ten, dia chi, avatar, phone
            return Json("true", JsonRequestBehavior.AllowGet);
        }

    }
}

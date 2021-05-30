using DoAnDiemDanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Web.Security;

namespace DoAnDiemDanh.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("HTTP404", "Account");
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult HTTP404()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            
            var tkgv = db.TAIKHOANGIANGVIENs.Where(s => s.TaiKhoan == model.UserName && s.MatKhau == model.Password).SingleOrDefault();
            var tksv = db.TAIKHOANSINHVIENs.Where(s => s.TaiKhoan == model.UserName && s.MatKhau == model.Password).SingleOrDefault();

            if(tkgv == null && tksv == null)
            {
                var tkgv1 = db.TAIKHOANGIANGVIENs.Where(s => s.TaiKhoan == model.UserName && s.MatKhau == model.Password).Single();
                var tksv1 = db.TAIKHOANSINHVIENs.Where(s => s.TaiKhoan == model.UserName && s.MatKhau == model.Password).Single();
            }

            else if (tkgv != null)
            {
                var gv = db.GIANGVIENs.Where(s => s.Email == model.UserName).SingleOrDefault();
                var tk = db.TAIKHOANGIANGVIENs.Where(s => s.TaiKhoan == model.UserName).SingleOrDefault();
                var str = "";
                if(tk.QUYEN.MaQuyen == 1)
                {
                    str = $"{gv.MaGV},{gv.TenGV},{"Admin"}";
                }
                else
                {
                    str = $"{gv.MaGV},{gv.TenGV},{"GV"}";
                }
                
                if (model.RememberMe)
                {
                    
                    var authTicket = new FormsAuthenticationTicket(2, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                    false, str);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName,FormsAuthentication.Encrypt(authTicket))
                    {
                        HttpOnly = true,
                        Expires = authTicket.Expiration
                    };

                    Response.AppendCookie(authCookie);
                }
                else
                {
                    var authTicket = new FormsAuthenticationTicket(2, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                    false, str);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
                    {
                        HttpOnly = true,
                       
                    };

                    Response.AppendCookie(authCookie);
                }
            
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Json(returnUrl,JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Url.Action("index","DiemDanh"), JsonRequestBehavior.AllowGet);
                }
            }
            else if(tksv != null)
            {
                var sv = db.SINHVIENs.Where(s => s.Email == model.UserName).SingleOrDefault();
                var str = $"{sv.MaSV},{sv.TenSV},{"SV"}";
                if (model.RememberMe)
                {

                    var authTicket = new FormsAuthenticationTicket(2, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                    false, str);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
                    {
                        HttpOnly = true,
                        Expires = authTicket.Expiration
                    };

                    Response.AppendCookie(authCookie);
                }
                else
                {
                    var authTicket = new FormsAuthenticationTicket(2, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                    false, str);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
                    {
                        HttpOnly = true,
                    };

                    Response.AppendCookie(authCookie);
                }

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Json(returnUrl, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Url.Action("XemDiemDanh", "SinhVien"), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(Url.Action("login", "Account"), JsonRequestBehavior.AllowGet);
            }
            return Json(Url.Action("login", "Account"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account"); ;
        }
    }
}
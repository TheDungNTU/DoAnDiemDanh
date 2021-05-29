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
            
            var tk = db.TAIKHOANs.Where(s => s.TaiKhoan1 == model.UserName && s.MatKhau == model.Password).Single();
            if(tk != null)
            {
                var gv = db.GIANGVIENs.Where(s => s.Email == model.UserName).SingleOrDefault();
           
                if (model.RememberMe)
                {
                    var authTicket = new FormsAuthenticationTicket(2, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                    false, gv.TenGV);

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
                    false, gv.TenGV);

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
            else
            {
                return Json(Url.Action("login", "Account"), JsonRequestBehavior.AllowGet);
            }                      
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account"); ;
        }
    }
}
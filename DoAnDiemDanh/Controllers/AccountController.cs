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
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            
            var tk = db.TAIKHOANs.Where(s => s.TaiKhoan1 == model.UserName && s.MatKhau == model.Password).SingleOrDefault();
            if(tk != null)
            {
                var gv = db.GIANGVIENs.Where(s => s.Email == model.UserName).SingleOrDefault();
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "DiemDanh"); 
                }
            }
            else
            {
                return View();
            }                      
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account"); ;
        }
    }
}
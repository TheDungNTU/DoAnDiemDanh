using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnDiemDanh.Controllers
{
    public class LayoutController : Controller
    {
        [ChildActionOnly]
        public ActionResult RenderUserInfo()
        {
            return PartialView("_LayoutUserInfo");
        }
    }
}
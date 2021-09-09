using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnDiemDanh.Models;

namespace DoAnDiemDanh.Controllers
{
    public class QuanLyCameraController : Controller
    {
      
        private BaseModel db = new BaseModel();

        public ActionResult Index()
        {
            return View(db.Entity.CAMERAs.ToList());
        }

        public ActionResult XemCamera()
        {
            return View(db.Entity.CAMERAs.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaCamera,TenCamera,RTSP,URL")] CAMERA cAMERA)
        {
            var cam = db.Entity.CAMERAs.SingleOrDefault(s => s.TenCamera == cAMERA.TenCamera && s.RTSP == cAMERA.RTSP);
            if (cam == null)
            {
                db.Entity.CAMERAs.Add(cAMERA);
                db.Entity.SaveChanges();
                return Json(cAMERA, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            CAMERA cAMERA = db.Entity.CAMERAs.Find(id);
            var data = new
            {
                id = id,
                TenCamera = cAMERA.TenCamera,
            };
            db.Entity.CAMERAs.Remove(cAMERA);
            db.Entity.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyCamera/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMERA cAMERA = db.Entity.CAMERAs.Find(id);
            if (cAMERA == null)
            {
                return HttpNotFound();
            }
            return View(cAMERA);
        }

        // POST: QuanLyCamera/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCamera,TenCamera,RTSP,URL")] CAMERA cAMERA)
        {
            if (ModelState.IsValid)
            {
                db.Entity.Entry(cAMERA).State = EntityState.Modified;
                db.Entity.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cAMERA);
        }

    
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

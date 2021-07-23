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
        private FACE_RECOGNITION_V2Entities db = new FACE_RECOGNITION_V2Entities();

        public ActionResult Index()
        {
            return View(db.CAMERAs.ToList());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaCamera,TenCamera,RTSP,URL")] CAMERA cAMERA)
        {
            var cam = db.CAMERAs.SingleOrDefault(s => s.TenCamera == cAMERA.TenCamera && s.RTSP == cAMERA.RTSP);
            if (cam == null)
            {
                db.CAMERAs.Add(cAMERA);
                db.SaveChanges();
                return Json(cAMERA, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            CAMERA cAMERA = db.CAMERAs.Find(id);
            var data = new
            {
                id = id,
                TenCamera = cAMERA.TenCamera,
            };
            db.CAMERAs.Remove(cAMERA);
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyCamera/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMERA cAMERA = db.CAMERAs.Find(id);
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
        public ActionResult Edit([Bind(Include = "MaCamera,RTSP,URL")] CAMERA cAMERA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cAMERA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cAMERA);
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

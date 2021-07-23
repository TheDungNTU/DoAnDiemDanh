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
    public class QuanLyPhongHocController : Controller
    {
        private FACE_RECOGNITION_V2Entities db = new FACE_RECOGNITION_V2Entities();

        // GET: QuanLyPhongHoc
        public ActionResult Index()
        {
            ViewBag.Camera = db.CAMERAs;
            var pHONGHOCs = db.PHONGHOCs.Include(p => p.CAMERA);
            return View(pHONGHOCs.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaPhongHoc,TenPhongHoc,MaCamera")] PHONGHOC pHONGHOC, string TenCamera)
        {
            var PhongHoc = db.PHONGHOCs.SingleOrDefault(s => s.TenPhongHoc == pHONGHOC.TenPhongHoc && s.MaCamera == pHONGHOC.MaCamera);

            if (PhongHoc == null)
            {
                db.PHONGHOCs.Add(pHONGHOC);
                db.SaveChanges();

                var myData = new
                {
                    MaPhongHoc = pHONGHOC.MaPhongHoc,
                    TenPhongHoc = pHONGHOC.TenPhongHoc,
                    TenCamera = TenCamera
                };
                return Json(myData, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }



        // POST: QuanLyLop/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            PHONGHOC Phong = db.PHONGHOCs.Find(id);
            var data = new
            {
                id = id,
                TenLop = Phong.TenPhongHoc
            };
            db.PHONGHOCs.Remove(Phong);
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONGHOC pHONGHOC = db.PHONGHOCs.Find(id);
            if (pHONGHOC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaCamera = new SelectList(db.CAMERAs, "MaCamera", "RTSP", pHONGHOC.MaCamera);
            return View(pHONGHOC);
        }

        // POST: QuanLyPhongHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaPhongHoc,TenPhongHoc,MaCamera")] PHONGHOC pHONGHOC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pHONGHOC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaCamera = new SelectList(db.CAMERAs, "MaCamera", "RTSP", pHONGHOC.MaCamera);
            return View(pHONGHOC);
        }

        // GET: QuanLyPhongHoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONGHOC pHONGHOC = db.PHONGHOCs.Find(id);
            if (pHONGHOC == null)
            {
                return HttpNotFound();
            }
            return View(pHONGHOC);
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

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
        private BaseModel db = new BaseModel();

        // GET: QuanLyPhongHoc
        public ActionResult Index()
        {
            ViewBag.Camera = db.Entity.CAMERAs;
            var pHONGHOCs = db.Entity.PHONGHOCs.Include(p => p.CAMERA);
            return View(pHONGHOCs.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaPhongHoc,TenPhongHoc,MaCamera")] PHONGHOC pHONGHOC, string TenCamera)
        {
            var PhongHoc = db.Entity.PHONGHOCs.SingleOrDefault(s => s.TenPhongHoc == pHONGHOC.TenPhongHoc && s.MaCamera == pHONGHOC.MaCamera);

            if (PhongHoc == null)
            {
                db.Entity.PHONGHOCs.Add(pHONGHOC);
                db.Entity.SaveChanges();

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
            PHONGHOC Phong = db.Entity.PHONGHOCs.Find(id);
            var data = new
            {
                id = id,
                TenLop = Phong.TenPhongHoc
            };
            db.Entity.PHONGHOCs.Remove(Phong);
            db.Entity.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONGHOC pHONGHOC = db.Entity.PHONGHOCs.Find(id);
            if (pHONGHOC == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaCamera = new SelectList(db.Entity.CAMERAs, "MaCamera", "RTSP", pHONGHOC.MaCamera);
            ViewBag.MaCam = db.Entity.PHONGHOCs.Where(x => x.MaPhongHoc == id).SingleOrDefault()?.MaCamera;
            ViewBag.TenPhong = db.Entity.PHONGHOCs.Where(x => x.MaPhongHoc == id).SingleOrDefault()?.TenPhongHoc;
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
                db.Entity.Entry(pHONGHOC).State = EntityState.Modified;
                db.Entity.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaCamera = new SelectList(db.Entity.CAMERAs, "MaCamera", "RTSP", pHONGHOC.MaCamera);
            return View(pHONGHOC);
        }

        // GET: QuanLyPhongHoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONGHOC pHONGHOC = db.Entity.PHONGHOCs.Find(id);
            if (pHONGHOC == null)
            {
                return HttpNotFound();
            }
            return View(pHONGHOC);
        }

        [HttpPost]
        public JsonResult CheckCamera(string MaCamera)
        {
            int Ma = Int32.Parse(MaCamera);
            var PhongCam = db.Entity.PHONGHOCs.Where(_ => _.MaCamera == Ma);

            if (PhongCam.Count() > 0)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckPhong(string TenPhongHoc)
        {
            string Ten = TenPhongHoc;
            var PhongCam = db.Entity.PHONGHOCs.Where(_ => _.TenPhongHoc == Ten);

            if (PhongCam.Count() > 0)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckCameraEdit(string MaCamera, int MaCamEdit)
        {

            var maCamEdit = MaCamEdit;
            // int maCamEdit = -1;//nt32.Parse(maCamEdit);
            //  var maCamEdit = Int32.Parse(ViewBag.MaCamEdit);

            int Ma = Int32.Parse(MaCamera);
            if (Ma == maCamEdit)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            var PhongCam = db.Entity.PHONGHOCs.Where(_ => _.MaCamera == Ma);

            if (PhongCam.Count() > 0)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckPhongHocEdit(string TenPhongHoc, string TenPhongHocEdit)
        {

            string Ten = TenPhongHoc;
            var TenPhongEdit = TenPhongHocEdit;// ViewBag.TenPhong;
            //TenPhongEdit = TenPhongEdit.ToString();
            if (Ten == TenPhongEdit)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            var PhongCam = db.Entity.PHONGHOCs.Where(_ => _.TenPhongHoc == Ten);

            if (PhongCam.Count() > 0)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
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

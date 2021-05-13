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
    public class QuanLyKhoaController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        // GET: QuanLyKhoa
        public ActionResult Index()
        {
            return View(db.KHOAs.ToList());
        }

        // GET: QuanLyKhoa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHOA kHOA = db.KHOAs.Find(id);
            if (kHOA == null)
            {
                return HttpNotFound();
            }
            return View(kHOA);
        }

        // GET: QuanLyKhoa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuanLyKhoa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaKhoa,TenKhoa")] KHOA kHOA)
        {
            if (ModelState.IsValid)
            {
                db.KHOAs.Add(kHOA);
                db.SaveChanges();
                return Json(kHOA, JsonRequestBehavior.AllowGet);
            }
            return Json("false",JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyKhoa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHOA kHOA = db.KHOAs.Find(id);
            if (kHOA == null)
            {
                return HttpNotFound();
            }
            return View(kHOA);
        }

        // POST: QuanLyKhoa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKhoa,TenKhoa")] KHOA kHOA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kHOA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kHOA);
        }

        // POST: QuanLyKhoa/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            KHOA kHOA = db.KHOAs.Find(id);
            db.KHOAs.Remove(kHOA);
            db.SaveChanges();
            return Json(id, JsonRequestBehavior.AllowGet);
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

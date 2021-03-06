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
    [Authorize(Roles = "Admin")]
    public class QuanLyLopController : Controller
    {
        private BaseModel db = new BaseModel();

        // GET: QuanLyLop
        public ActionResult Index()
        {
            ViewBag.MaKhoa = db.Entity.KHOAs;
            var lOPs = db.Entity.LOPs.Include(l => l.KHOA);
            return View(lOPs.ToList());
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaLop,TenLop,MaKhoa")] LOP lOP, string TenKhoa)
        {
            var Lop = db.Entity.LOPs.SingleOrDefault(s => s.TenLop == lOP.TenLop && s.MaKhoa == lOP.MaKhoa);
            if (Lop == null)
            {
                db.Entity.LOPs.Add(lOP);
                db.Entity.SaveChanges();

                var myData = new
                {
                    MaLop = lOP.MaLop,
                    TenLop = lOP.TenLop,
                    MaKhoa = lOP.MaKhoa,
                    TenKhoa = TenKhoa
                };

                return Json(myData, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // GET: QuanLyLop/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOP lOP = db.Entity.LOPs.Find(id);
            if (lOP == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.Entity.KHOAs, "MaKhoa", "TenKhoa", lOP.MaKhoa);
            return View(lOP);
        }

        // POST: QuanLyLop/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLop,TenLop,MaKhoa")] LOP lOP)
        {
            if (ModelState.IsValid)
            {
                db.Entity.Entry(lOP).State = EntityState.Modified;
                db.Entity.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.Entity.KHOAs, "MaKhoa", "TenKhoa", lOP.MaKhoa);
            return View(lOP);
        }


        // POST: QuanLyLop/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            LOP lOP = db.Entity.LOPs.Find(id);
            var data = new
            {
                id = id,
                TenLop = lOP.TenLop
            };
            db.Entity.LOPs.Remove(lOP);
            db.Entity.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
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

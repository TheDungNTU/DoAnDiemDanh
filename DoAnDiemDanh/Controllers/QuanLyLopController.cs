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
    public class QuanLyLopController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        // GET: QuanLyLop
        public ActionResult Index()
        {

            var t = (from lophoc in db.LOPs
                    select new LopHocCount{malh = lophoc.MaLop, soluong = lophoc.SINHVIENs.Count() }).ToList();
            ViewBag.danhsach = t;


            ViewBag.MaKhoa = db.KHOAs;
            var lOPs = db.LOPs.Include(l => l.KHOA);
            return View(lOPs.ToList());
        }

        // GET: QuanLyLop/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOP lOP = db.LOPs.Find(id);
            if (lOP == null)
            {
                return HttpNotFound();
            }
            return View(lOP);
        }

        // GET: QuanLyLop/Create
        public ActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa");
            return View();
        }

        // POST: QuanLyLop/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MaLop,TenLop,MaKhoa")] LOP lOP, string TenKhoa)
        {
            if (ModelState.IsValid)
            {
                db.LOPs.Add(lOP);
                db.SaveChanges();

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
            LOP lOP = db.LOPs.Find(id);
            if (lOP == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", lOP.MaKhoa);
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
                db.Entry(lOP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", lOP.MaKhoa);
            return View(lOP);
        }

        // GET: QuanLyLop/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOP lOP = db.LOPs.Find(id);
            if (lOP == null)
            {
                return HttpNotFound();
            }
            return View(lOP);
        }

        // POST: QuanLyLop/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            LOP lOP = db.LOPs.Find(id);
            db.LOPs.Remove(lOP);
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

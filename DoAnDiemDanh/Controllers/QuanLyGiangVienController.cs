﻿using System;
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
    public class QuanLyGiangVienController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        // GET: QuanLyGiangVien
        public ActionResult Index()
        {

            var gIANGVIENs = db.GIANGVIENs.Include(g => g.KHOA);
            ViewBag.MONHOC = db.MONHOCs.Include(_ => _.GIANGVIEN);
            return View(gIANGVIENs.ToList());
        }

        // GET: QuanLyGiangVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);
            if (gIANGVIEN == null)
            {
                return HttpNotFound();
            }
            return View(gIANGVIEN);
        }

        // GET: QuanLyGiangVien/Create
        public ActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa");
            return View();
        }

        // POST: QuanLyGiangVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaGV,TenGV,Password,MaKhoa")] GIANGVIEN gIANGVIEN)
        {
            if (ModelState.IsValid)
            {
                db.GIANGVIENs.Add(gIANGVIEN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
            return View(gIANGVIEN);
        }

        // GET: QuanLyGiangVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);
            if (gIANGVIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
            return View(gIANGVIEN);
        }

        // POST: QuanLyGiangVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaGV,TenGV,Password,MaKhoa")] GIANGVIEN gIANGVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gIANGVIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKhoa = new SelectList(db.KHOAs, "MaKhoa", "TenKhoa", gIANGVIEN.MaKhoa);
            return View(gIANGVIEN);
        }

        // GET: QuanLyGiangVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);
            if (gIANGVIEN == null)
            {
                return HttpNotFound();
            }
            return View(gIANGVIEN);
        }

        // POST: QuanLyGiangVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GIANGVIEN gIANGVIEN = db.GIANGVIENs.Find(id);
            db.GIANGVIENs.Remove(gIANGVIEN);
            db.SaveChanges();
            return RedirectToAction("Index");
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

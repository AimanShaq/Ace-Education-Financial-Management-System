using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using Ace_Tuition_WBL.Models;
using EntityState = System.Data.Entity.EntityState;

namespace Ace_Tuition_WBL.Controllers
{
    public class SecondaryController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Secondary
        [HandleError]
        public ActionResult Index()
        {
            return View(db.tbSecondaries.ToList());
        }

        // GET: Secondary/Details
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSecondary tbSecondary = db.tbSecondaries.Find(id);
            if (tbSecondary == null)
            {
                return HttpNotFound();
            }
            return View(tbSecondary);
        }

        // GET: Secondary/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Secondary/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SecondaryID,SecondaryFee,SecondaryMaterial")] tbSecondary tbSecondary)
        {
            if (ModelState.IsValid)
            {
                tbSecondary.CatID = 2;
                db.tbSecondaries.Add(tbSecondary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbSecondary);
        }

        // GET: Secondary/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSecondary tbSecondary = db.tbSecondaries.Find(id);
            if (tbSecondary == null)
            {
                return HttpNotFound();
            }
            return View(tbSecondary);
        }

        // POST: Secondary/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SecondaryID,SecondaryFee,SecondaryMaterial")] tbSecondary tbSecondary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbSecondary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbSecondary);
        }

        // GET: Secondary/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSecondary tbSecondary = db.tbSecondaries.Find(id);
            if (tbSecondary == null)
            {
                return HttpNotFound();
            }
            return View(tbSecondary);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbSecondary tbSecondary = db.tbSecondaries.Find(id);
            db.tbSecondaries.Remove(tbSecondary);
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
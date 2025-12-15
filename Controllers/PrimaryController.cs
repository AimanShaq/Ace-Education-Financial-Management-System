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
    public class PrimaryController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Primary
        [HandleError]
        public ActionResult Index()
        {
            return View(db.tbPrimaries.ToList());
        }

        // GET: Primary/Details
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPrimary tbPrimary = db.tbPrimaries.Find(id);
            if (tbPrimary == null)
            {
                return HttpNotFound();
            }
            return View(tbPrimary);
        }

        // GET: Primary/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Primary/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrimaryID,PrimaryFee,PrimaryMaterial")] tbPrimary tbPrimary)
        {
            if (ModelState.IsValid)
            {
                tbPrimary.CatID = 1;
                db.tbPrimaries.Add(tbPrimary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbPrimary);
        }

        // GET: Primary/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPrimary tbPrimary = db.tbPrimaries.Find(id);
            if (tbPrimary == null)
            {
                return HttpNotFound();
            }
            return View(tbPrimary);
        }

        // POST: Primary/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrimaryID,PrimaryFee,PrimaryMaterial")] tbPrimary tbPrimary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbPrimary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbPrimary);
        }

        // GET: Primary/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPrimary tbPrimary = db.tbPrimaries.Find(id);
            if (tbPrimary == null)
            {
                return HttpNotFound();
            }
            return View(tbPrimary);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbPrimary tbPrimary = db.tbPrimaries.Find(id);
            db.tbPrimaries.Remove(tbPrimary);
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
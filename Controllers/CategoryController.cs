using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ace_Tuition_WBL.Models;
using EntityState = System.Data.Entity.EntityState;

namespace Ace_Tuition_WBL.Controllers
{
    public class CategoryController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Category
        [HandleError]
        public ActionResult Index()
        {
            return View(db.tbCategories.ToList());
        }

        // GET: Category/Details/5
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategory tbCategory = db.tbCategories.Find(id);
            if (tbCategory == null)
            {
                return HttpNotFound();
            }
            return View(tbCategory);
        }

        // GET: Category/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CatID,CatDesc")] tbCategory tbCategory)
        {
            if (ModelState.IsValid)
            {
                db.tbCategories.Add(tbCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbCategory);
        }

        // GET: Category/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategory tbCategory = db.tbCategories.Find(id);
            if (tbCategory == null)
            {
                return HttpNotFound();
            }
            return View(tbCategory);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CatID,CatDesc")] tbCategory tbCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbCategory);
        }

        // GET: Category/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCategory tbCategory = db.tbCategories.Find(id);
            if (tbCategory == null)
            {
                return HttpNotFound();
            }
            return View(tbCategory);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbCategory tbCategory = db.tbCategories.Find(id);
            db.tbCategories.Remove(tbCategory);
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
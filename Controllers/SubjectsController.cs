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
    public class SubjectsController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Subjects
        [HandleError]
        public ActionResult Index()
        {
            return View(db.tbSubjects.ToList());
        }

        // GET: Subjects/Details/5
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSubject tbSubject = db.tbSubjects.Find(id);
            if (tbSubject == null)
            {
                return HttpNotFound();
            }
            return View(tbSubject);
        }

        // GET: Subjects/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubjectNames,CatID,SubjectCodes,SubjectNameC")] tbSubject tbSubject)
        {
            if (ModelState.IsValid)
            {
                db.tbSubjects.Add(tbSubject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbSubject);
        }

        // GET: Subjects/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSubject tbSubject = db.tbSubjects.Find(id);
            if (tbSubject == null)
            {
                return HttpNotFound();
            }
            return View(tbSubject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubjectNames,CatID,SubjectID,SubjectCodes,SubjectNameC")] tbSubject tbSubject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbSubject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbSubject);
        }

        // GET: Subjects/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbSubject tbSubject = db.tbSubjects.Find(id);
            if (tbSubject == null)
            {
                return HttpNotFound();
            }
            return View(tbSubject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbSubject tbSubject = db.tbSubjects.Find(id);
            db.tbSubjects.Remove(tbSubject);
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

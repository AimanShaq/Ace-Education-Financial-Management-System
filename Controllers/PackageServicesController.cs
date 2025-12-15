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
    public class PackageServicesController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: PackageServices
        [HandleError]
        public ActionResult Index()
        {
            return View(db.tbPackages.ToList());
        }

        // GET: PackageServices/Details/5
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPackage tbPackage = db.tbPackages.Find(id);
            if (tbPackage == null)
            {
                return HttpNotFound();
            }
            return View(tbPackage);
        }

        // GET: PackageServices/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PackageServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageStat,PackageName,PackageDesc,PackageFee")] tbPackage tbPackage)
        {
            if (ModelState.IsValid)
            {
                db.tbPackages.Add(tbPackage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbPackage);
        }

        // GET: PackageServices/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPackage tbPackage = db.tbPackages.Find(id);
            if (tbPackage == null)
            {
                return HttpNotFound();
            }
            return View(tbPackage);
        }

        // POST: PackageServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageStat,PackageName,PackageDesc,PackageFee")] tbPackage tbPackage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbPackage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbPackage);
        }

        // GET: PackageServices/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPackage tbPackage = db.tbPackages.Find(id);
            if (tbPackage == null)
            {
                return HttpNotFound();
            }
            return View(tbPackage);
        }

        // POST: PackageServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbPackage tbPackage = db.tbPackages.Find(id);
            db.tbPackages.Remove(tbPackage);
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

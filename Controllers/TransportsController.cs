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
    public class TransportsController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Transports
        [HandleError]
        public ActionResult Index()
        {
            return View(db.tbTransports.ToList());
        }

        // GET: Transports/Details/5
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbTransport tbTransport = db.tbTransports.Find(id);
            if (tbTransport == null)
            {
                return HttpNotFound();
            }
            return View(tbTransport);
        }

        // GET: Transports/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Transports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "transDPW,transName,transFee")] tbTransport tbTransport)
        {
            if (ModelState.IsValid)
            {
                db.tbTransports.Add(tbTransport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbTransport);
        }

        // GET: Transports/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbTransport tbTransport = db.tbTransports.Find(id);
            if (tbTransport == null)
            {
                return HttpNotFound();
            }
            return View(tbTransport);
        }

        // POST: Transports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "transDPW,transName,transFee")] tbTransport tbTransport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbTransport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbTransport);
        }

        // GET: Transports/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbTransport tbTransport = db.tbTransports.Find(id);
            if (tbTransport == null)
            {
                return HttpNotFound();
            }
            return View(tbTransport);
        }

        // POST: Transports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbTransport tbTransport = db.tbTransports.Find(id);
            db.tbTransports.Remove(tbTransport);
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

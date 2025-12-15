using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ace_Tuition_WBL.Models;
using Ace_Tuition_WBL.ViewModel;
using EntityState = System.Data.Entity.EntityState;
//using Ace_Tuition_WBL.Repository;

namespace Ace_Tuition_WBL.Controllers
{
    public class ParentController : Controller
    {
        private readonly Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Parent
        [HandleError]
        public ActionResult Index()
        {
            var lists = new StudentParent()
            {
                parentlist = db.tbParents.ToList(),
                studentlist = db.tbStudents.ToList(),
            };
            return View(lists);
        }

        [HandleError]
        public ActionResult Child(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbParent tbParent = db.tbParents.Find(id);
            if (tbParent == null)
            {
                return HttpNotFound();
            }
            ViewData["id"] = id;
            var parentname = tbParent.ParentName;
            ViewData["Name"] = parentname; 

            List<tbStudent> studpar = db.tbStudents.ToList();
            List<tbParent> parent = db.tbParents.ToList();

            var childlist = from s in studpar
                            join p in parent on s.ParentID equals p.ID into table1
                            where s.ParentID == id
                            from p in table1.DefaultIfEmpty()
                            select new StudentParent { studentdetail = s, parentdetail = p };
            
            return View(childlist);
        }

        [HandleError]
        public ActionResult AddChild(int? id)
        {
            ViewData["id"] = id;
            return View();
        }
        [HttpPost]
        [HandleError]
        public ActionResult AddChild([Bind(Include = "StudentID,ParentID,StudentName,StudentCat,StudentContact,subjCount,transDPW,ccStat,mealStat,writingStat,StudentIC,StudentNameC,StudentStandard")] tbStudent tbStudent)
        {

            if (ModelState.IsValid)
            {
                tbStudent.subjCount = 0;
                tbStudent.ccStat = 0;
                tbStudent.mealStat = 0;
                tbStudent.transDPW = 0;
                tbStudent.writingStat = 0;
                tbStudent.StudentStatus = 1;

                db.tbStudents.Add(tbStudent);
                db.SaveChanges();
                



                tbParent tbPar = db.tbParents.Find(tbStudent.ParentID);
                List<Ace_Tuition_WBL.Models.tbStudent> liststud = db.tbStudents.ToList();
                tbPar.siblingCount = liststud.Count(x => x.ParentID == tbPar.ID);
                db.Entry(tbPar).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("EditReg", "Student", new { id = tbStudent.StudentID });
            }

            return RedirectToAction("Child", "Child", new { id = tbStudent.ParentID});
        }

        // GET: Student/Details/5
        [HandleError]
        public ActionResult DetailsChild(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbStudent tbStudent = db.tbStudents.Find(id);
            if (tbStudent == null)
            {
                return HttpNotFound();
            }
            return View(tbStudent);
        }
        // GET: Student/Edit/5
        [HandleError]
        public ActionResult EditChild(int? id, int? pid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbStudent tbStudent = db.tbStudents.Find(id);
            if (tbStudent == null)
            {
                return HttpNotFound();
            }
            ViewData["pid"] = pid;
            return View(tbStudent);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult EditChild([Bind(Include = "StudentID,ParentID,StudentName,StudentCat,StudentContact,subjCount,transDPW,ccStat,mealStat,writingStat,StudentIC,StudentNameC,StudentStandard, StudentStatus")] tbStudent tbStudent)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(tbStudent).State = EntityState.Modified;
                //db.Entry(tbStudent).CurrentValues.SetValues(tbStudent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["pid"] = tbStudent.ParentID;
            return View(tbStudent);
        }

        // GET: Student/Delete/5
        [HandleError]
        public ActionResult DeleteChild(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbStudent tbStudent = db.tbStudents.Find(id);
            if (tbStudent == null)
            {
                return HttpNotFound();
            }
            return View(tbStudent);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("DeleteChild")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteChildConfirmed(int id)
        {
            
            var outstd = db.tbOutstandings.Where(x => x.StudentID == id);
            var rec = db.tbReceipts.Where(u => u.StudentID == id);
            var regii = db.tbRegistrations.Where(u => u.StudentID == id);

            db.tbReceipts.RemoveRange(rec);
            db.tbRegistrations.RemoveRange(regii);
            db.tbOutstandings.RemoveRange(outstd);

            tbStudent tbStudent = db.tbStudents.Find(id);
            db.tbStudents.Remove(tbStudent);
            db.SaveChanges();

            tbParent tbPar = db.tbParents.Find(tbStudent.ParentID);
            List<Ace_Tuition_WBL.Models.tbStudent> liststud = db.tbStudents.ToList();
            tbPar.siblingCount = liststud.Count(x => x.ParentID == tbPar.ID);
            db.Entry(tbPar).State = EntityState.Modified;
            db.SaveChanges();

            


            return RedirectToAction("Index");
        }

        // GET: Parent/Details/5
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbParent tbParent = db.tbParents.Find(id);
            if (tbParent == null)
            {
                return HttpNotFound();
            }
            return View(tbParent);
        }

        // GET: Parent/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parent/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentName,Password,ParentContact,B40Status,siblingCount,ParentAdress,ParentIC,ParentReg,ParentNameC,ParentEmail")] tbParent tbParent)
        {
            if (ModelState.IsValid)
            {
                tbParent.ParentReg = 1;
                tbParent.siblingCount = 0;
                db.tbParents.Add(tbParent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbParent);
        }

        // GET: Parent/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbParent tbParent = db.tbParents.Find(id);

            if (tbParent == null)
            {
                return HttpNotFound();
            }
            return View(tbParent);
        }

        // POST: Parent/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ParentName,Password,ParentContact,B40Status,ParentIC,ParentAdress,siblingCount,ParentNameC,ParentEmail,ParentReg")] tbParent tbParent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbParent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbParent);
        }

        // GET: Parent/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbParent tbParent = db.tbParents.Find(id);
            if (tbParent == null)
            {
                return HttpNotFound();
            }
            return View(tbParent);
        }

        // POST: Parent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbParent tbParent = db.tbParents.Find(id);
            List<tbStudent> student = db.tbStudents.Where(x => x.ParentID == id).ToList();

            foreach(var child in student)
            {
                var regii = db.tbRegistrations.Where(u => u.StudentID == child.StudentID).ToList();
                if(regii != null) { 
                    db.tbRegistrations.RemoveRange(regii);
                }

            }

            List<tbOutstanding> outstd = db.tbOutstandings.Where(x => x.ParentID == id).ToList();
            var tbStudent = db.tbStudents.Where(s => s.ParentID == id).ToList();
            var rec = db.tbReceipts.Where(u => u.ParentID == id).ToList();

            //var regii = db.tbRegistrations.Where(u => u.StudentID == );
            db.tbOutstandings.RemoveRange(outstd);
            db.tbReceipts.RemoveRange(rec);
            db.tbStudents.RemoveRange(tbStudent);
            db.tbParents.Remove(tbParent);
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

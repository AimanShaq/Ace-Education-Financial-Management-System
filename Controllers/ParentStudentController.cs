using Ace_Tuition_WBL.Models;
using Ace_Tuition_WBL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Ace_Tuition_WBL.Controllers
{
    public class ParentStudentController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: ParentStudent
        [HandleError]
        public ActionResult Index()
        {

            var sessionassign = Session["ParentID"];
            int parentid;

            //int parentid = Convert.ToInt32(Session["ParentID"]);
            List<tbStudent> studpar = db.tbStudents.ToList();
            List<tbParent> parent = db.tbParents.ToList();

            tbParent par = parent.Where(x => x.ParentIC == (string)sessionassign || x.ParentEmail == (string)sessionassign).FirstOrDefault();
            parentid = par.ID;

            var childlist = from s in studpar
                            join p in parent on s.ParentID equals p.ID into table1
                            where s.ParentID == parentid
                            from p in table1.DefaultIfEmpty()
                            select new StudentParent { studentdetail = s, parentdetail = p };

            return View(childlist);
        }

        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //StudentDetailsViewModel stu = db.tbStudents.Find(id);
            tbStudent tbStudent = db.tbStudents.Find(id);
            var name = tbStudent.StudentName;
            ViewData["Name"] = name;
            ViewData["id"] = id;
            if (tbStudent == null)
            {
                return HttpNotFound();
            }
            return View(tbStudent);
        }
    }
}
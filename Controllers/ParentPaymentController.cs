using Ace_Tuition_WBL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.IO;
using Ace_Tuition_WBL.ViewModel;
using System.Web.UI;

namespace Ace_Tuition_WBL.Controllers
{
    public class ParentPaymentController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: ParentPayment
        [HandleError]
        public ActionResult Index()
        {
            var sessionassign = Session["ParentID"];

            int parentid;// = Convert.ToInt32(Session["ParentID"]);
            List<tbReceipt> receipt = db.tbReceipts.ToList();
            List<tbParent> parent = db.tbParents.ToList();

            tbParent par = parent.Where(x => x.ParentIC == (string)sessionassign || x.ParentEmail == (string)sessionassign).FirstOrDefault();

            parentid = par.ID;

            var receiptlist = from r in receipt
                              join p in parent on r.ParentID equals p.ID into table1
                              where r.ParentID == parentid && r.ReceiptStatus != 2
                              from p in table1.DefaultIfEmpty()
                              select new ParentPaymentViewModel { receiptdetails = r };

            return View(receiptlist);
        }

        [HandleError]
        public ActionResult History()
        {
            var sessionassign = Session["ParentID"];
            List<tbParent> parent = db.tbParents.ToList();

            tbParent par = parent.Where(x => x.ParentIC == (string)sessionassign || x.ParentEmail == (string)sessionassign).FirstOrDefault();

            int ids = par.ID;

            return View(db.tbReceipts.ToList().Where(x => x.ReceiptStatus == 2 && x.ParentID == ids));
        }

        // GET: ParentPayment/Details/5
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbReceipt tbReceipt = db.tbReceipts.Find(id);
            if (tbReceipt == null)
            {
                return HttpNotFound();
            }
            float tuition = 0;
            float material = 0;
            float reg = 0;
            float early = 0;
            float early1 = 0;
            float package = (float)(tbReceipt.tbStudent.tbPackage1.PackageFee + tbReceipt.tbStudent.tbPackage.PackageFee);
            float trans = (float)(tbReceipt.tbStudent.tbTransport.transFee);

            if (tbReceipt.tbStudent.tbParent.ParentReg == 1)
            {
                reg = 30;
            }
            if (tbReceipt.earlyPay == 1)
            {
                if (tbReceipt.tbStudent.ccStat == 0)
                {
                    early = 10;
                }
                else
                {
                    early1 = 30;
                }
            }
            if (tbReceipt.tbStudent.StudentCat == 1)
            {
                foreach (var item in tbReceipt.tbStudent.tbCategory.tbPrimaries.ToList())
                {
                    if (tbReceipt.tbStudent.subjCount == item.PrimaryID)
                    {
                        tuition = tuition + (float)(item.PrimaryFee);
                        material = material + (float)(item.PrimaryMaterial);
                    }
                }
            }
            if (tbReceipt.tbStudent.StudentCat == 2)
            {
                foreach (var item in tbReceipt.tbStudent.tbCategory.tbSecondaries.ToList())
                {
                    if (tbReceipt.tbStudent.subjCount == item.SecondaryID)
                    {
                        tuition = tuition + (float)(item.SecondaryFee);
                        material = material + (float)(item.SecondaryMaterial);
                    }
                }
            }
            ViewData["subtotal"] = tuition + material + reg + package + trans;
            ViewData["early"] = early;
            ViewData["earlycc"] = early1;
            return View(tbReceipt);
        }

        // GET: ParentPayment/Details/5
        [HandleError]
        public ActionResult HistoryDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            tbReceipt tbReceipt = db.tbReceipts.Find(id);
            if (tbReceipt == null)
            {
                return HttpNotFound();
            }
            return View(tbReceipt);
        }

        // GET: ParentPayment/Create
        [HttpGet]
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ParentPayment/Create
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParentPaymentViewModel parentPaymentViewModel)
        {
            
                using(db)
                {
                    tbReceipt tbReceipt = new tbReceipt();

                    tbReceipt.ReceiptID = parentPaymentViewModel.ReceiptID;
                    tbReceipt.ParentID = parentPaymentViewModel.ParentID;
                    tbReceipt.ReceiptTotal = parentPaymentViewModel.ReceiptTotal;
                    tbReceipt.ReceiptAmount = parentPaymentViewModel.ReceiptAmount;
                    tbReceipt.StudentID = parentPaymentViewModel.StudentID;
                    tbReceipt.ReceiptDate = parentPaymentViewModel.ReceiptDate;
                    tbReceipt.ReceiptDiscount = parentPaymentViewModel.ReceiptDiscount;
                    tbReceipt.ReceiptDesc = parentPaymentViewModel.ReceiptDesc;
                    tbReceipt.ReceiptProof = SaveToPhysicalLocation(parentPaymentViewModel.ReceiptProof);
                    tbReceipt.ReceiptStatus = parentPaymentViewModel.ReceiptStatus;
                    
                    db.tbReceipts.Add(tbReceipt);
                    db.SaveChanges();
                }
            return RedirectToAction("Index");
        }

        // GET: ParentPayment/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbReceipt tbReceipt = db.tbReceipts.Find(id);
            if (tbReceipt == null)
            {
                return HttpNotFound();
            }

            float tuition = 0;
            float material = 0;
            float reg = 0;
            float early = 0;
            float early1 = 0;
            float package = (float)(tbReceipt.tbStudent.tbPackage1.PackageFee + tbReceipt.tbStudent.tbPackage.PackageFee);
            float trans = (float)(tbReceipt.tbStudent.tbTransport.transFee);

            if (tbReceipt.tbStudent.tbParent.ParentReg == 1)
            {
                reg = 30;
            }
            if (tbReceipt.earlyPay == 1)
            {
                if (tbReceipt.tbStudent.ccStat == 0)
                {
                    early = 10;
                }
                else
                {
                    early1 = 30;
                }
            }
            if (tbReceipt.tbStudent.StudentCat == 1)
            {
                foreach (var item in tbReceipt.tbStudent.tbCategory.tbPrimaries.ToList())
                {
                    if (tbReceipt.tbStudent.subjCount == item.PrimaryID)
                    {
                        tuition = tuition + (float)(item.PrimaryFee);
                        material = material + (float)(item.PrimaryMaterial);
                    }
                }
            }
            if (tbReceipt.tbStudent.StudentCat == 2)
            {
                foreach (var item in tbReceipt.tbStudent.tbCategory.tbSecondaries.ToList())
                {
                    if (tbReceipt.tbStudent.subjCount == item.SecondaryID)
                    {
                        tuition = tuition + (float)(item.SecondaryFee);
                        material = material + (float)(item.SecondaryMaterial);
                    }
                }
            }
            ViewData["subtotal"] = tuition + material + reg + package + trans;
            ViewData["early"] = early;
            ViewData["earlycc"] = early1;

            return View(tbReceipt);
        }

        // POST: ParentPayment/Edit/5
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, ParentPaymentViewModel parentPaymentViewModel)
        {
            using(db)
            {
                string guid = Guid.NewGuid().ToString();

                string pathofthefile = guid + Path.GetExtension(parentPaymentViewModel.ReceiptProof.FileName);

                tbReceipt tbReceipt = db.tbReceipts.Find(id);

                if (tbReceipt.ReceiptProof != null)
                {
                    string filePath = tbReceipt.ReceiptProof;

                    string fullpath = Request.MapPath(tbReceipt.ReceiptProof);
                    System.IO.File.Delete(fullpath);

                    parentPaymentViewModel.ReceiptProof.SaveAs(Server.MapPath("~/Uploads/" + pathofthefile));
                    tbReceipt.ReceiptProof = "~/Uploads/"+pathofthefile;
                    tbReceipt.ReceiptStatus = 1;
                }
                else
                {
                    parentPaymentViewModel.ReceiptProof.SaveAs(Server.MapPath("~/Uploads/" + pathofthefile));
                    tbReceipt.ReceiptProof = "~/Uploads/" + pathofthefile;
                    tbReceipt.ReceiptStatus = 1;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: ParentPayment/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbReceipt tbReceipt = db.tbReceipts.Find(id);
            if (tbReceipt == null)
            {
                return HttpNotFound();
            }
            return View(tbReceipt);
        }

        // POST: ParentPayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeletetedConfirmed(int id)
        {
            tbReceipt tbReceipt = db.tbReceipts.Find(id);
            string filePath = tbReceipt.ReceiptProof;
            System.IO.File.Delete(filePath);
            db.tbReceipts.Remove(tbReceipt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private string SaveToPhysicalLocation(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var user = (string)Session["ParentName"]; 
                var date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                file.SaveAs(path);
                return path;
            }
            return string.Empty;
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

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
    public class PaymentController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Payment
        [HandleError]
        public ActionResult Index()
        {
            var tbReceipts = db.tbReceipts.Include(t => t.tbStudent).Where(x => x.tbStudent.StudentStatus == 1).ToList();

            DateTime datenow = DateTime.Now;
            DateTime firstdayofmonth = new DateTime(datenow.Year, datenow.Month, 1);
            DateTime lastdiscount = new DateTime(datenow.Year, datenow.Month, 15);

            var liststud = db.tbStudents.ToList();

            foreach (var receipt in tbReceipts)
            {
                int receiptdateMonth = DateTime.Parse(receipt.BillDate.ToString()).Month;
                //tbStudent selectedstud = new tbStudent();
                //tbReceipt receiptupd2 = new tbReceipt();
                tbReceipt receiptupd = db.tbReceipts.Where(x => x.StudentID == receipt.StudentID && x.BillDate.Value.Month == datenow.Month).FirstOrDefault();

                //kalau receipt untuk bulan sama dengan bulan sekarang tak wujud
                if (receiptupd != null)
                {
                    tbStudent thestud = db.tbStudents.Find(receipt.StudentID);
                    float totalreceipt = 0;
                    //******************************    Calculation      ************************************
                    if (receipt.ReceiptStatus == 0)
                    {
                        if (thestud.StudentCat == 1) //if student category = primary
                        {
                            tbPrimary studprimary = db.tbPrimaries.Find(thestud.subjCount); //subjcount == id tbPrimary
                            if (studprimary is null)
                                totalreceipt = 0;
                            else
                                totalreceipt = (float)(studprimary.PrimaryFee + studprimary.PrimaryMaterial); //subjcount+primaryfee total
                        }
                        else //if student category = secondary
                        {
                            tbSecondary studsecondary = db.tbSecondaries.Find(thestud.subjCount); //subjcount == id tbsecondary
                            if (studsecondary is null)
                                totalreceipt = 0;
                            else
                                totalreceipt = (float)(studsecondary.SecondaryFee + studsecondary.SecondaryMaterial); //subjcount+secondaryfee total
                        }
                        if (thestud.tbParent.siblingCount > 1) //if parent has many childrens
                        {
                            totalreceipt = totalreceipt - (float)(0.05 * totalreceipt); //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total - 5% sibling
                        }
                        if (thestud.tbParent.B40Status != 0) //jic dont want to 0/100 would be error
                        {
                            totalreceipt = totalreceipt - (float)((float)(thestud.tbParent.B40Status) / 100 * totalreceipt);
                            //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total - 5% sibling - %b40discount 
                        }

                        //Transport
                        tbTransport studtransport = db.tbTransports.Find(thestud.transDPW); //transdpw == id tbtransport
                        totalreceipt = totalreceipt + (float)(studtransport.transFee); //subjcount + transdpw total
                        //Package
                        tbPackage studcc = db.tbPackages.Find(thestud.ccStat);          //same thing
                        tbPackage studmeal = db.tbPackages.Find(thestud.mealStat);
                        tbPackage studwrit = db.tbPackages.Find(thestud.writingStat);
                        totalreceipt = totalreceipt + (float)(studcc.PackageFee + studmeal.PackageFee + studwrit.PackageFee); //subjcount + transdpw + meal+ cc - 30 for early total

                        if (thestud.tbParent.ParentReg == 1) //if parent firstreg
                        {
                            receipt.firstReg = 1;
                            thestud.tbParent.ParentReg = 0;
                        }
                        if(receipt.firstReg == 1)
                        {
                            totalreceipt = totalreceipt + 30; //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total
                        }

                        if (datenow < lastdiscount && receipt.BillDate.Value.Month == datenow.Month) //for early bird ----------------------------------------------------------------------------------------
                        {
                            receipt.earlyPay = 1;
                            if (studcc.PackageStat != 0)
                            {
                                totalreceipt = totalreceipt - 20; //-20 if early bird and took childcare
                            }
                            totalreceipt = totalreceipt - 10; //-10 for earlybird
                                                                //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total - 5% sibling - %b40discount - 10 earlybird
                        }

                        else //for late bird ----------------------------------------------------------------------------------------
                        {
                            receipt.earlyPay = 0;
                            //totalreceipt = totalreceipt - 10; //no -10 for latebird
                        }
                    }
                    else
                    {
                        totalreceipt = (float)receipt.ReceiptTotal;
                    }
                    receipt.ReceiptTotal = Math.Round((double)totalreceipt,2);
                    //***************************************************************************************

                    //kalau tak bayar lagi date akan terus update
                    if (receipt.ReceiptStatus != 2)
                    {
                        receipt.ReceiptDate = datenow;
                    }
                    //Change null to 0
                    if (receipt.ReceiptAmount == null)
                    {
                        receipt.ReceiptAmount = 0;
                    }

                    //receiptupd.ReceiptID = receipt.ReceiptID;
                    //if (datenow < lastdiscount)
                    //    receiptupd.earlyPay = 1;
                    //else
                    //    receiptupd.earlyPay = 0;
                    //receiptupd.firstReg = thestud.tbParent.ParentReg;
                    //receiptupd.ReceiptTotal = totalreceipt;

                    //*******************************************      Chaeck Outstanding      ***********************************

                    tbOutstanding outstand = db.tbOutstandings.Where(x => x.StudentID == receipt.StudentID).FirstOrDefault();
                    if(outstand != null && outstand.OutStatus == 2)
                    {
                        receipt.ReceiptAmount = receipt.ReceiptAmount - outstand.OutFee;
                    }

                    //*******************************************    Outstanding Calculation     ********************************************

                    //Kalau tak bayar or amount bayar tak sama dengan harga payment
                    if (receipt.ReceiptAmount != receipt.ReceiptTotal)
                    {
                        //tbOutstanding outstandinglist = db.tbOutstandings.Where(x => x.StudentID == receipt.StudentID).FirstOrDefault();

                        //**************** kalau outstanding takde *******************
                        if (outstand == null)
                        {
                            tbOutstanding outstanding = new tbOutstanding();
                            outstanding.StudentID = receipt.StudentID;
                            outstanding.ParentID = receipt.ParentID;
                            var balance = (float)receipt.ReceiptTotal - (float)receipt.ReceiptAmount;
                            outstanding.OutFee = Math.Round((double)balance, 2);
                            outstanding.OutMonth = receiptdateMonth;
                            db.tbOutstandings.Add(outstanding);
                            db.SaveChanges();
                        }
                        else
                        {
                            //supposedly pegang id by parent id untuk allow parent bayar sekali for all child but SCRAP THAT
                            tbOutstanding outstandinglist = db.tbOutstandings.Where(x => x.ParentID == receipt.ParentID).FirstOrDefault();
                            tbOutstanding outstandinged = db.tbOutstandings.Find(outstandinglist.OutID);

                            //**************** kalau bayaran kurang *******************
                            if(receipt.ReceiptAmount < receipt.ReceiptTotal)
                            {
                                var balance = (float)receipt.ReceiptTotal - (float)receipt.ReceiptAmount;
                                outstand.OutFee = Math.Round((double)balance, 2);
                                outstand.OutStatus = 1;
                                db.Entry(outstand).State = EntityState.Modified; ;
                                db.SaveChanges();

                                if(receipt.ReceiptStatus != 1)
                                {
                                    receipt.ReceiptStatus = 0;
                                }
                                receipt.ReceiptOutstanding = 1;
                            }
                            //**************** kalau bayaran cukup *******************
                            else if (receipt.ReceiptAmount == receipt.ReceiptTotal)
                            {
                                db.tbOutstandings.Remove(outstand);
                                db.SaveChanges(); 
                                
                                receipt.ReceiptAmount = receipt.ReceiptTotal;
                                receipt.ReceiptStatus = 2;
                                receipt.ReceiptOutstanding = 0;
                            }
                            //**************** kalau bayaran lebih *******************
                            else
                            {
                                var balance = (float)receipt.ReceiptTotal - (float)receipt.ReceiptAmount;
                                outstand.OutFee = Math.Round((double)balance, 2);
                                outstand.OutStatus = 2;
                                db.Entry(outstand).State = EntityState.Modified; ;
                                db.SaveChanges();

                                receipt.ReceiptAmount = receipt.ReceiptTotal;
                                receipt.ReceiptStatus = 2;
                                receipt.ReceiptOutstanding = 2;
                            }
                        }

                    }
                    //If payment total & amount pay is equal to one other status changed to accepted 
                    else
                    {
                        receipt.ReceiptStatus = 2;
                        if (receipt.ReceiptOutstanding == 1)
                        {
                            db.tbOutstandings.Remove(outstand);
                            db.SaveChanges();
                            receipt.ReceiptOutstanding = 0;
                        }
                    }

                    db.Entry(receipt).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    receiptupd = new tbReceipt();
                    tbStudent thestud = db.tbStudents.Find(receipt.StudentID);
                    float totalreceipt = 0;
                    //if (receipt.ReceiptStatus == 0)
                    //{
                    if (thestud.StudentCat == 1) //if student category = primary
                    {
                        tbPrimary studprimary = db.tbPrimaries.Find(thestud.subjCount); //subjcount == id tbPrimary
                        if (studprimary is null)
                            totalreceipt = 0;
                        else
                            totalreceipt = (float)(studprimary.PrimaryFee + studprimary.PrimaryMaterial); //subjcount+primaryfee total
                    }
                    else //if student category = secondary
                    {
                        tbSecondary studsecondary = db.tbSecondaries.Find(thestud.subjCount); //subjcount == id tbsecondary
                        if (studsecondary is null)
                            totalreceipt = 0;
                        else
                            totalreceipt = (float)(studsecondary.SecondaryFee + studsecondary.SecondaryMaterial); //subjcount+secondaryfee total
                    }

                    tbTransport studtransport = db.tbTransports.Find(thestud.transDPW); //transdpw == id tbtransport
                    totalreceipt = totalreceipt + (float)(studtransport.transFee); //subjcount + transdpw total

                    tbPackage studcc = db.tbPackages.Find(thestud.ccStat);          //same thing
                    tbPackage studmeal = db.tbPackages.Find(thestud.mealStat);
                    tbPackage studwrit = db.tbPackages.Find(thestud.writingStat);
                    totalreceipt = totalreceipt + (float)(studcc.PackageFee + studmeal.PackageFee + studwrit.PackageFee); //subjcount + transdpw + meal+ cc - 30 for early total

                    if (receipt.firstReg == 1) //if parent firstreg
                    {
                        totalreceipt = totalreceipt + 30; //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total
                    }

                    if (thestud.tbParent.siblingCount > 1) //if parent has many childrens
                    {
                        totalreceipt = totalreceipt - (float)(0.05 * totalreceipt); //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total - 5% sibling
                    }
                    if (thestud.tbParent.B40Status != 0) //jic dont want to 0/100 would be error
                    {
                        totalreceipt = totalreceipt - (float)((float)(thestud.tbParent.B40Status) / 100 * totalreceipt);
                        //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total - 5% sibling - %b40discount 
                    }

                    if (datenow < lastdiscount) //for early bird ----------------------------------------------------------------------------------------
                    {
                        receiptupd.earlyPay = 1;
                        if (studcc.PackageStat != 0)
                        {
                            totalreceipt = totalreceipt - 20; //-20 if early bird and took childcare
                        }
                        totalreceipt = totalreceipt - 10; //-10 for earlybird
                                                            //subjcount + transdpw + meal+ cc - 30 for early  + 30 for first reg total - 5% sibling - %b40discount - 10 earlybird
                    }
                    else //for late bird ----------------------------------------------------------------------------------------
                    {
                        receiptupd.earlyPay = 0;
                        //totalreceipt = totalreceipt - 10; //no -10 for latebird
                    }
                    //}
                    //else
                    //{
                    //    totalreceipt = (float)receipt.ReceiptTotal;
                    //}
                    if (datenow < lastdiscount)
                        receiptupd.earlyPay = 1;
                    else
                        receiptupd.earlyPay = 0;
                    //receiptupd.ReceiptID = receipt.ReceiptID;
                    receiptupd.StudentID = receipt.StudentID;
                    receiptupd.ParentID = thestud.tbParent.ID;
                    receiptupd.ReceiptDate = datenow;
                    receiptupd.ReceiptStatus = 0;
                    receiptupd.ReceiptOutstanding = 1;
                    receiptupd.firstReg = thestud.tbParent.ParentReg;
                    receiptupd.ReceiptTotal = Math.Round((double)totalreceipt, 2);
                    receiptupd.BillDate = firstdayofmonth;

                    db.tbReceipts.Add(receiptupd);
                    db.SaveChanges();

                    // Lepas save first reg price on first child change status to 0 so that second child don,t have to pay first reg price
                    //thestud.tbParent.ParentReg = 0;
                    //db.Entry(thestud).State = EntityState.Modified;
                    //db.SaveChanges();
                }

                
            }
            if(TempData["Message"]!= null) { 
                ViewBag.Message = TempData["Message"].ToString();
            }

            return View(tbReceipts);
        }

        [HandleError]
        [HttpPost]
        public ActionResult Index(int Month)
        {
            if(Month == 0)
            {
                List<tbReceipt> receipt = db.tbReceipts.ToList();
                return View(receipt);
            }
            else
            {
                List<tbReceipt> receipt = db.tbReceipts.Where(x => x.BillDate.Value.Month == Month).ToList();
                return View(receipt);
            }
        }

        // GET: Payment/Details/5
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
            float outstand = 0;
            float package = (float)(tbReceipt.tbStudent.tbPackage1.PackageFee + tbReceipt.tbStudent.tbPackage.PackageFee + tbReceipt.tbStudent.tbPackage2.PackageFee);
            float trans = (float)(tbReceipt.tbStudent.tbTransport.transFee);

            if (tbReceipt.firstReg == 1)
            {
                reg = 30;
            }
            if (tbReceipt.earlyPay == 1)
            {
                if(tbReceipt.tbStudent.ccStat == 0)
                {
                    early = 10;
                }
                else
                {
                    early1 = 30;
                }
            }
            if(tbReceipt.tbStudent.StudentCat == 1)
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
            outstand = (float)(tbReceipt.ReceiptTotal - tbReceipt.ReceiptAmount);
            ViewData["outstand"] = outstand;
            ViewData["subtotal"] = tuition + material + reg + package + trans;
            ViewData["early"] = early;
            ViewData["earlycc"] = early1;
            return View(tbReceipt);
        }

        // GET: Payment/Create
        [HandleError]
        public ActionResult Create()
        {
            ViewBag.ReceiptID = new SelectList(db.tbStudents, "StudentID", "StudentName");
            ViewBag.StudentID = new SelectList(db.tbStudents, "StudentID", "StudentName");
            return View();
        }

 

        // POST: Payment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceiptID,ParentID,ReceiptTotal,ReceiptAmount,StudentID,ReceiptDate,ReceiptDiscount,ReceiptDesc,ReceiptProof,earlyPay,firstReg,ReceiptStatus")] tbReceipt tbReceipt)
        {
            if (ModelState.IsValid)
            {
                db.tbReceipts.Add(tbReceipt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReceiptID = new SelectList(db.tbStudents, "StudentID", "StudentName", tbReceipt.ReceiptID);
            ViewBag.StudentID = new SelectList(db.tbStudents, "StudentID", "StudentName", tbReceipt.StudentID);
            return View(tbReceipt);
        }

        // GET: Payment/Edit/5
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
            float outstand = 0;
            outstand = (float)(tbReceipt.ReceiptTotal - tbReceipt.ReceiptAmount);
            ViewData["outstand"] = outstand;
            ViewBag.ReceiptID = new SelectList(db.tbStudents, "StudentID", "StudentName", tbReceipt.ReceiptID);
            ViewBag.StudentID = new SelectList(db.tbStudents, "StudentID", "StudentName", tbReceipt.StudentID);
            return View(tbReceipt);
        }

        // POST: Payment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbReceipt tbReceipt)
        {

            if (ModelState.IsValid)
            {
                tbReceipt tbReceipt1 = db.tbReceipts.Where(x => x.StudentID == tbReceipt.StudentID && x.BillDate == tbReceipt.BillDate).FirstOrDefault();

                tbReceipt1.ReceiptID = tbReceipt.ReceiptID;
                tbReceipt1.ParentID = tbReceipt.ParentID;
                tbReceipt1.ReceiptTotal = tbReceipt.ReceiptTotal;
                tbReceipt1.StudentID = tbReceipt.StudentID;
                tbReceipt1.ReceiptDate = tbReceipt.ReceiptDate;
                tbReceipt1.ReceiptDiscount = tbReceipt.ReceiptDiscount;
                tbReceipt1.ReceiptDesc = tbReceipt.ReceiptDesc;
                tbReceipt1.ReceiptProof = tbReceipt.ReceiptProof;
                tbReceipt1.earlyPay = tbReceipt.earlyPay;
                tbReceipt1.firstReg = tbReceipt.firstReg;
                tbReceipt1.ReceiptStatus = tbReceipt.ReceiptStatus;
                tbReceipt1.ReceiptOutstanding = tbReceipt.ReceiptOutstanding;
                //tambah amount baru dengan yg lama
                if(tbReceipt.ReceiptOutstanding == 1)
                {
                    tbReceipt1.ReceiptAmount = Math.Round((double)(tbReceipt1.ReceiptAmount + tbReceipt.ReceiptAmount),2);
                }
                else
                {
                    tbReceipt1.ReceiptAmount = Math.Round((double)(tbReceipt.ReceiptAmount),2);
                }

                db.Entry(tbReceipt1).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.ReceiptID = new SelectList(db.tbStudents, "StudentID", "StudentName", tbReceipt.ReceiptID);
            ViewBag.StudentID = new SelectList(db.tbStudents, "StudentID", "StudentName", tbReceipt.StudentID);
            return View();
        }

        // GET: Payment/Delete/5
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

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbReceipt tbReceipt = db.tbReceipts.Find(id);
            db.tbReceipts.Remove(tbReceipt);
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

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

namespace Ace_Tuition_WBL.Controllers
{
    public class StudentController : Controller
    {
        private readonly Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Student
        [HandleError]
        public ActionResult Index()
        {
            var lists = new SubjectReg()
            {
                StudentgetList = db.tbStudents.ToList(),
                RegistrationList = db.tbRegistrations.ToList(),
            };
            return View(lists);
        }

        [HandleError]
        public ActionResult Status(int? id, int stat)
        {
            tbStudent tbStudent = db.tbStudents.Find(id);
            tbStudent.StudentStatus = stat;
            db.Entry(tbStudent).State = EntityState.Modified; 
            db.SaveChanges();
            //var lists = new SubjectReg()
            //{
            //    StudentgetList = db.tbStudents.ToList(),
            //    RegistrationList = db.tbRegistrations.ToList(),
            //};
            return RedirectToAction("Index");
        }
        // GET: Student/Details/5
        [HandleError]
        public ActionResult Details(int? id)
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
        //----------------------------------------------------------------------------------------------------------
        // GET: Student/Create
        [HandleError]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentID,ParentID,StudentName,StudentCat,StudentContact,subjCount,transDPW,ccStat,mealStat,writingStat")] tbStudent tbStudent)
        {
            if (ModelState.IsValid)
            {
                db.tbStudents.Add(tbStudent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbStudent);
        }
        //--------------------------------------------------------------------------------------------------------
        // GET: Student/Edit/5
        [HandleError]
        public ActionResult Edit(int? id)
        {

            SubjectModel subjects = new SubjectModel();
            

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

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID,ParentID,StudentName,StudentCat,StudentContact,subjCount,ccStat,mealStat,transDPW,writingStat")] tbStudent tbStudent)
        {
            if (ModelState.IsValid)
            {
                tbStudent thastud = db.tbStudents.Where(x => x.StudentID == tbStudent.StudentID).FirstOrDefault();

                tbStudent.StudentID = tbStudent.StudentID;
                tbStudent.ParentID = tbStudent.ParentID;
                tbStudent.ccStat = thastud.ccStat;
                tbStudent.mealStat = thastud.mealStat;
                tbStudent.transDPW = thastud.transDPW;
                tbStudent.writingStat = thastud.writingStat;
                //db.Entry(tbStudent).CurrentValues.SetValues(tbStudent) /*= EntityState.Modified*/;
                db.Entry(tbStudent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbStudent);
        }
        //------------------------------------------------------------------------------------------
        [HandleError]
        public ActionResult EditReg(int? id)
        {
            SubjectModel subjects = new SubjectModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbStudent tbStudent = db.tbStudents.Find(id);
            List<tbRegistration>reg = db.tbRegistrations.Where(x => x.StudentID == id).ToList();

            if (tbStudent == null)
            {
                return HttpNotFound();
            }

            var both = new SubjectModel()
            {
                Subjectlist = db.tbSubjects.ToList(),
                Studentget = tbStudent,
                reglist = reg
            };


            return View(both);

        }
        //[Bind(Include = "RegID,SubjectCode,StudentID")] tbRegistration tbRegistration
        [HttpPost]
        [HandleError]
        [ValidateAntiForgeryToken]
        public ActionResult EditReg(SubjectModel model)
        {
            DeleteRegConfirmed(model.Studentget.StudentID);
            var selectedsubj = model.Subjectlist.Where(x => x.IsChecked == true).ToList<tbSubject>();
            var studid = model.Studentget.StudentID;
           
            tbRegistration tbReg = new tbRegistration();
            if (ModelState.IsValid)
            {
                for (int i =0; i< selectedsubj.Count; i++) { 
                
                    tbReg.StudentID = studid;
                    tbReg.SubjectCode = selectedsubj[i].SubjectCodes;
                    db.tbRegistrations.Add(tbReg);
                    db.SaveChanges();
                    
                }

                tbStudent tbStud = db.tbStudents.Find(model.Studentget.StudentID);
                tbStud.subjCount = selectedsubj.Count();
                tbStud.mealStat = model.Studentget.mealStat;
                tbStud.ccStat = model.Studentget.ccStat;
                tbStud.transDPW = model.Studentget.transDPW;
                tbStud.writingStat = model.Studentget.writingStat;


                db.Entry(tbStud).State = EntityState.Modified;
                db.SaveChanges();

                var tbReceipts = db.tbReceipts.Include(t => t.tbStudent).ToList();

                DateTime datenow = DateTime.Now;
                DateTime firstdayofmonth = new DateTime(datenow.Year, datenow.Month, 1);
                DateTime lastdiscount = new DateTime(datenow.Year, datenow.Month, 15);

                List<tbStudent> liststud = db.tbStudents.ToList();
                var parentid = tbStud.ParentID;


                tbReceipt receipt = new tbReceipt();
                receipt.StudentID = studid;
                receipt.ReceiptDate = datenow;
                receipt.ReceiptStatus = 0;
                receipt.ParentID = parentid;
                int receiptdateMonth = DateTime.Parse(receipt.ReceiptDate.ToString()).Month;
                //tbStudent selectedstud = new tbStudent();
                //tbReceipt receiptupd2 = new tbReceipt();
                tbReceipt receiptupd = db.tbReceipts.Where(x => x.StudentID == receipt.StudentID).FirstOrDefault();

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
                        if (receipt.firstReg == 1)
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
                    receipt.ReceiptTotal = Math.Round((double)totalreceipt, 2);
                    //*******************************************************************************

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

                    //*******************************************      Chaeck Outstanding      ***********************************

                    tbOutstanding outstand = db.tbOutstandings.Where(x => x.StudentID == receipt.StudentID).FirstOrDefault();
                    if (outstand != null && outstand.OutStatus == 2)
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
                            if (receipt.ReceiptAmount < receipt.ReceiptTotal)
                            {
                                var balance = (float)receipt.ReceiptTotal - (float)receipt.ReceiptAmount;
                                outstand.OutFee = Math.Round((double)balance, 2);
                                outstand.OutStatus = 1;
                                db.Entry(outstand).State = EntityState.Modified; ;
                                db.SaveChanges();

                                receipt.ReceiptStatus = 0;
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
                    receipt.BillDate = receiptupd.BillDate;
                    receipt.firstReg = receiptupd.firstReg;
                    //receipt.ReceiptID = receiptupd.ReceiptID;
                    db.Entry(receiptupd).State = EntityState.Modified;
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


                return RedirectToAction("Index");
            }
            //return RedirectToAction("Index");
            return View(tbReg);
        }


        //------------------------------------------------------------------------------------------
        // GET: Student/Delete/5
        [HandleError]
        public ActionResult Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteConfirmed(int id)
        {
            tbStudent tbStudent = db.tbStudents.Find(id);
            db.tbStudents.Remove(tbStudent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //------------------------------------------------------------------------------------------
        // GET: Student/Delete/5
        [HandleError]
        public ActionResult DeleteReg(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var regListDel = db.tbRegistrations.Where(x => x.StudentID == id).ToList();

            tbRegistration tbRegistration = new tbRegistration();

            for (int i = 0; i < regListDel.Count(); i++)
            {
                tbRegistration.StudentID = id;
                tbRegistration.SubjectCode = regListDel[i].SubjectCode;
            }
            if (tbRegistration == null)
            {
                return HttpNotFound();
            }
            return View(tbRegistration);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("DeleteReg")]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult DeleteRegConfirmed(int id)
        {

            var regListDel = db.tbRegistrations.Where(x => x.StudentID == id).ToList();

            for (int i = 0; i < regListDel.Count; i++)
            {
                //tbRegistr.RegID = regListDel[i].RegID;
                var regid = regListDel[i].RegID;

                tbRegistration tbRegistr = db.tbRegistrations.Find(regid);
                db.tbRegistrations.Remove(tbRegistr);
               
            }
            db.SaveChanges();

            var regListAfterDel = db.tbRegistrations.Where(x => x.StudentID == id).ToList();
            tbStudent tbStud = db.tbStudents.Find(id);
            tbStud.subjCount = regListAfterDel.Count();
            db.Entry(tbStud).State = EntityState.Modified;
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

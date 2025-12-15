using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using Ace_Tuition_WBL.Models;

namespace Ace_Tuition_WBL.Controllers
{
    public class EmailController : Controller
    {
        private Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();

        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        // GET: Email/Details/5
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
           
                    string subject = form["Subject"];
                    string message = form["Body"];
                    message = message.Replace(System.Environment.NewLine, "<br>");

                    //AceducationDummy1!
                    var senderEmail = new MailAddress("aceducationdummy1@gmail.com", "Ace Education");
                    //var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "AceducationDummy1!";
                    var sub = subject;
                    var body = message;


                    //SmtpClient smtp = new SmtpClient();
                    

                    List<tbParent> listparent = db.tbParents.ToList();
                    List<tbOutstanding> listoutstandings = db.tbOutstandings.ToList();
                    List<tbReceipt> listrec = db.tbReceipts.ToList();

                    foreach (var par in listparent)  //check receipts
                    {
                         foreach (var rec in listrec.Where(x=>x.ParentID == par.ID))//check parents receipts
                        {
                            foreach(var outd in listoutstandings.Where(x => x.StudentID == rec.StudentID)) //check outstanding parents receipts
                            {
                                float tuition = 0;
                            float material = 0;
                            float reg = 0;
                            float early = 0;
                            float earlycc = 0;
                            float package = (float)(rec.tbStudent.tbPackage1.PackageFee + rec.tbStudent.tbPackage.PackageFee + rec.tbStudent.tbPackage2.PackageFee);
                            float trans = (float)(rec.tbStudent.tbTransport.transFee);

                            if (rec.tbStudent.tbParent.ParentReg == 1)
                            {
                                reg = 30;
                            }
                            if (rec.earlyPay == 1)
                            {
                                if(rec.tbStudent.ccStat == 0)
                                {
                                    early = 10;
                                }
                                else
                                {
                                    earlycc = 30;
                                }
                            }
                            if(rec.tbStudent.StudentCat == 1)
                            {
                                foreach (var item in rec.tbStudent.tbCategory.tbPrimaries.ToList())
                                {
                                    if (rec.tbStudent.subjCount == item.PrimaryID)
                                    {
                                        tuition = tuition + (float)(item.PrimaryFee);
                                        material = material + (float)(item.PrimaryMaterial);
                                    }
                                }
                            }
                            if (rec.tbStudent.StudentCat == 2)
                            {
                                foreach (var item in rec.tbStudent.tbCategory.tbSecondaries.ToList())
                                {
                                    if (rec.tbStudent.subjCount == item.SecondaryID)
                                    {
                                        tuition = tuition + (float)(item.SecondaryFee);
                                        material = material + (float)(item.SecondaryMaterial);
                                    }
                                }
                            }
                            var subtotal = tuition + material + reg + package + trans;
                            


                                string outbody = "";
                                outbody += "<strong> Student Name: </strong>";
                                outbody += "<br>"+rec.tbStudent.StudentName;
                                outbody += "<strong> Student IC: </strong>";
                                outbody += "<br>"+rec.tbStudent.StudentIC;
                                outbody = "<style>" +
                                "td, th { border: 1px solid black; }" +
                                "</style>";

                                outbody += "<table style='border:1px solid black;'>";
                                outbody += "<thead><tr>" +
                                    "<th>Item</th>" +
                                    "<th>Total (RM)</th>" +
                                    "</tr></thead>" +
                                    "<tbody>";
                                outbody+= "<tr>";
                                if (rec.firstReg == 1) { outbody += "<td>Registration Fee:</td><td>30.00</td>"; } 
                                outbody += "</tr>" ;
                                outbody+= "<tr>";
                                outbody += "<td> Tuition Fee ( " + rec.tbStudent.subjCount + " Subjects )</td>";
                                if (rec.tbStudent.StudentCat == 1) {
                                    foreach (var item in rec.tbStudent.tbCategory.tbPrimaries.ToList())
                                    {
                                        if (rec.tbStudent.subjCount == item.PrimaryID)
                                        {
                                            outbody+= "<td>"+String.Format("{0:0.00}", item.PrimaryFee)+"</td>";
                                        }
                                    }
                                }
                                if (rec.tbStudent.StudentCat == 2)
                                {
                                    foreach (var item in rec.tbStudent.tbCategory.tbSecondaries.ToList())
                                    {
                                        if (rec.tbStudent.subjCount == item.SecondaryID)
                                        {
                                            outbody+= "<td>"+String.Format("{0:0.00}", item.SecondaryFee)+"</td>";
                                        }

                                    }
                                }
                                outbody += "</tr>" ;
                                outbody+= "<tr>";
                                outbody += "<td> Material Fee</td>";
                                if (rec.tbStudent.StudentCat == 1)
                                    {
                                        foreach (var item in rec.tbStudent.tbCategory.tbPrimaries.ToList())
                                        {
                                            if (rec.tbStudent.subjCount == item.PrimaryID)
                                            {
                                                outbody+= "<td>"+String.Format("{0:0.00}", item.PrimaryMaterial)+"</td>";
                                            }
                                        }
                                    }
                                    if (rec.tbStudent.StudentCat == 2)
                                    {
                                        foreach (var item in rec.tbStudent.tbCategory.tbSecondaries.ToList())
                                        {
                                            if (rec.tbStudent.subjCount == item.SecondaryID)
                                            {
                                                outbody+= "<td>"+String.Format("{0:0.00}", item.SecondaryMaterial)+"</td>";
                                            }
                                        }
                                    }
                                outbody += "</tr>" ;
                                if (rec.tbStudent.ccStat != 0)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td> Childcare </ td >";
                                    outbody += "<td>" + String.Format("{0:0.00}", rec.tbStudent.tbPackage.PackageFee) + "</td>";
                                    outbody += "</tr>";
                                }
                                if (rec.tbStudent.mealStat != 0)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td> Meal </td>";
                                    outbody += "<td>" + String.Format("{0:0.00}", rec.tbStudent.tbPackage1.PackageFee) + "</td>";
                                    outbody += "</tr>";
                                }
                                if (rec.tbStudent.writingStat != 0)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td> Writing </td>";
                                    outbody += "<td>" + String.Format("{0:0.00}", rec.tbStudent.tbPackage2.PackageFee) + "</td>";
                                    outbody += "</tr>";
                                }
                                if (rec.tbStudent.transDPW != 0)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td> Transport </td>";
                                    outbody += "<td>" + String.Format("{0:0.00}", rec.tbStudent.tbTransport.transFee) + "</td>";
                                    outbody += "</tr>";
                                }
                                outbody+= "<tr>";
                                outbody += "<td><strong>Subtotal (RM):<strong></td>" +
                                        "<td>"+ String.Format("{0:0.00}", subtotal) + "</td>"; 
                                outbody += "</tr>" ;
                                if (rec.tbStudent.tbParent.B40Status != 0)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td><strong>Discount: </strong></td>";
                                    outbody += "<td> -" + rec.tbStudent.tbParent.B40Status +"%";
                                    outbody += "</td>" + "</tr>";
                                }     
                                if (rec.earlyPay == 1)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td><strong> Early Pay Discount: </strong></td>";
                                    outbody += "<td>";
                                    if (rec.tbStudent.ccStat == 0)
                                    {
                                        outbody += "<p>- " + String.Format("{0:0.00}", early) + "</p>";
                                    }
                                    else
                                    {
                                        outbody += "<p>- " + String.Format("{0:0.00}", earlycc) + "</p>";
                                    }
                                    outbody += "</td>" + "</tr>";        
                                }
                                if (rec.tbStudent.tbParent.siblingCount > 1)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td><strong>Sibling Discount: </strong></td>";
                                    outbody += "<td> -5%";
                                    outbody += "</td>" + "</tr>";
                                }
                                outbody+= "<tr>";
                                outbody += "<td><strong>Total (RM):<strong></td>" +
                                        "<td>"+ String.Format("{0:0.00}", rec.ReceiptTotal) + "</td>"; 
                                outbody += "</tr>" ;
                                if (rec.ReceiptAmount != null)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td style='color:green'><strong>Paid: </strong></td>";
                                    outbody += "<td>" + String.Format("{0:0.00}", rec.ReceiptAmount);
                                    outbody += "</td>" + "</tr>";
                                }
                                if (rec.ReceiptOutstanding == 1)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td style='color:red'><strong>Outstanding (RM): </strong></td>";
                                    outbody += "<td>";
                                    foreach (var item in rec.tbStudent.tbOutstandings.ToList())
                                        {
                                            if (item.StudentID == rec.tbStudent.StudentID)
                                            {
                                                outbody+= String.Format("{0:0.00}", item.OutFee);
                                            }
                                        }
                                    outbody += "</td>" + "</tr>";
                                }
                                if (rec.ReceiptAmount == null)
                                {
                                    outbody += "<tr>";
                                    outbody += "<td style='color:red'><strong>Outstanding (RM): </strong></td>";
                                    outbody += "<td style='color:red'>" + String.Format("{0:0.00}", rec.ReceiptTotal);
                                    outbody += "</td>" + "</tr>";
                                }

                                outbody+="</tbody>";
                                outbody += "</table>";
                                
                                if(par.ParentEmail != null)
                                {
                                    using (var mess = new MailMessage())
                                    {
                                        mess.From = new MailAddress(senderEmail.Address, "Ace Education");
                                        mess.To.Add(par.ParentEmail);
                                        mess.Subject = subject;
                                        mess.Body =  "<p>"+body+"</p>" +"\n <h3>Outstanding:</h3> \n" +outbody; //+ outbody
                                        mess.IsBodyHtml = true;
                            
                                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                        {
                                            //smtp.UseDefaultCredentials = false;
                                            smtp.Credentials = new NetworkCredential(senderEmail.Address, password);
                                            smtp.EnableSsl = true;
                                            smtp.Send(mess);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    
                    TempData["Message"] = "Email sent successfully";

                return RedirectToAction("Index","Payment");


        }
    }
}

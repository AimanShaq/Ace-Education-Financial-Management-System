using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ace_Tuition_WBL.Models;
using System.Net.Mail;
using System.Net;

namespace Ace_Tuition_WBL.Controllers
{
    public class HomeController : Controller
    {
        private readonly Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();
        // GET: Account
        [HttpGet]
        [HandleError]
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            return View();
        }
        [HttpPost]
        [HandleError]
        public ActionResult Index(tbAdmin log1 )
        {
            var idcheck = db.tbParents.FirstOrDefault(x => x.ParentIC == log1.ID.ToString());
            var emailcheck = db.tbParents.FirstOrDefault(x => x.ParentEmail == log1.ID.ToString());

            if (idcheck != null)
            {
                if (log1.ID != null)
                {
                    var userPass = db.tbParents.Where(x => x.ParentIC == log1.ID.ToString()).Select(x => x.Password);
                    //Materializes the query into a readable list of array objects
                    var materializePass = userPass.ToList();
                    //Since query will only result to one , we ony have one array of result
                    var password = materializePass[0];

                    var username = db.tbParents.Where(x => x.ParentIC == log1.ID.ToString()).Select(x => x.ParentName);
                    var materializePass1 = username.ToList();
                    var name = materializePass1[0];

                    tbParent parentname = db.tbParents.Where(x => x.ParentIC == log1.ID.ToString()).FirstOrDefault();
                    

                    //Check username and password (Case Sensitive)
                    if (idcheck.ParentIC == log1.ID.ToString() && log1.Password == password)
                    {
                        Session["ParentID"] = log1.ID;
                        //Session["ParentName"] = parentname.ParentName;
                        if(name != null)
                        {
                            Session["ParentName"] = name;
                        }
                        else
                        {
                            Session["ParentName"] = "Not found";
                        }
                        //Your code here to redirect
                        return RedirectToAction("ParentDashBoard");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid id and password";
                        return View("Index", log1);
                    }
                }

            }
            if (emailcheck != null)
            {
                if (log1.ID != null)
                {
                    var userPass = db.tbParents.Where(x => x.ParentEmail == log1.ID.ToString()).Select(x => x.Password);
                    //Materializes the query into a readable list of array objects
                    var materializePass = userPass.ToList();
                    //Since query will only result to one , we ony have one array of result
                    var password = materializePass[0];

                    var username = db.tbParents.Where(x => x.ParentEmail == log1.ID.ToString()).Select(x => x.ParentName);
                    var materializePass1 = username.ToList();
                    var name = materializePass1[0];

                    tbParent parentname = db.tbParents.Where(x => x.ParentEmail == log1.ID.ToString()).FirstOrDefault();


                    //Check username and password (Case Sensitive)
                    if (emailcheck.ParentEmail == log1.ID.ToString() && log1.Password == password)
                    {
                        Session["ParentID"] = log1.ID;
                        //Session["ParentName"] = parentname.ParentName;
                        if (name != null)
                        {
                            Session["ParentName"] = name;
                        }
                        else
                        {
                            Session["ParentName"] = "Not found";
                        }
                        //Your code here to redirect
                        return RedirectToAction("ParentDashBoard");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid id and password";
                        return View("Index", log1);
                    }
                }

            }


            var idcheck1 = db.tbAdmins.FirstOrDefault(x => x.ID == log1.ID);
            if (idcheck1 != null)
            {
                if (log1.ID != null)
                {
                    var userPass = db.tbAdmins.Where(x => x.ID == log1.ID).Select(x => x.Password);
                    //Materializes the query into a readable list of array objects
                    var materializePass = userPass.ToList();
                    //Since query will only result to one , we ony have one array of result
                    var password = materializePass[0];

                    //Check username and password (Case Sensitive)
                    if (idcheck1.ID == log1.ID && log1.Password == password)
                    {
                        Session["User_Id"] = log1.ID;
                        //Your code here to redirect
                        return RedirectToAction("AdminDashBoard");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid id and password";
                        return View("Index", log1);
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid id and password";
                return View("Index", log1);
            }
            return View();
        }
        //public ActionResult Index(tbParent log)
        //{
        //    var idcheck = db.tbParents.FirstOrDefault(x => x.ID == log.ID);
        //    if (idcheck != null)
        //    {
        //        if (log.ID != 0)
        //        {
        //            var userPass = db.tbParents.Where(x => x.ID == log.ID).Select(x => x.Password);
        //            //Materializes the query into a readable list of array objects
        //            var materializePass = userPass.ToList();
        //            //Since query will only result to one , we ony have one array of result
        //            var password = materializePass[0];

        //            //Check username and password (Case Sensitive)
        //            if (idcheck.ID == log.ID && log.Password == password)
        //            {
        //                Session["ParentID"] = log.ID;
        //                //Your code here to redirect
        //                return RedirectToAction("ParentDashBoard");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ErrorMessage = "Invalid id and password";
        //        return View("Index", log);
        //    }
        //    return View();
        //}
        [HandleError]
        public ActionResult ParentDashBoard()
        {
            return View();
        }
        [HandleError]
        public ActionResult AdminDashBoard()
        {
	        //Revenue per month
            double jan = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 1 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double feb = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 2 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double mac = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 3 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double apr = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 4 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double may = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 5 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double june = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 6 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double july = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 7 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double aug = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 8 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double sept = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 9 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double oct = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 10 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double nov = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 11 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();
            double dis = (double)db.tbReceipts.Where(x => x.ReceiptStatus == 2 && x.BillDate.Value.Month == 12 && x.BillDate.Value.Year == DateTime.Today.Year && x.ReceiptAmount != null).Sum(x => x.ReceiptTotal).GetValueOrDefault();


            ViewData["jan"] = jan;
            ViewData["feb"] = feb;
            ViewData["mac"] = mac;
            ViewData["apr"] = apr;
            ViewData["may"] = may;
            ViewData["june"] = june;
            ViewData["july"] = july;
            ViewData["aug"] = aug;
            ViewData["sept"] = sept;
            ViewData["oct"] = oct;
            ViewData["nov"] = nov;
            ViewData["dis"] = dis;

            //Outstanding per month
            double out_jan = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 1 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_feb = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 2 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_mac = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 3 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_apr = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 4 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_may = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 5 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_june = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 6 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_july = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 7 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_aug = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 8 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_sept = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 9 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_oct = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 10 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_nov = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 11 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();
            double out_dis = (double)db.tbOutstandings.Where(x => x.OutStatus == 1 && x.OutMonth == 12 && x.OutFee > 0).Sum(x => x.OutFee).GetValueOrDefault();


            ViewData["out_jan"] = out_jan;
            ViewData["out_feb"] = out_feb;
            ViewData["out_mac"] = out_mac;
            ViewData["out_apr"] = out_apr;
            ViewData["out_may"] = out_may;
            ViewData["out_june"] = out_june;
            ViewData["out_july"] = out_july;
            ViewData["out_aug"] = out_aug;
            ViewData["out_sept"] = out_sept;
            ViewData["out_oct"] = out_oct;
            ViewData["out_nov"] = out_nov;
            ViewData["out_dis"] = out_dis;

            return View();
        }
        [HandleError]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
        [HandleError]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HandleError]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // GET: Account
        [HttpGet]
        [HandleError]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [HandleError]
        public ActionResult ForgotPassword(tbAdmin log1)
        {
            //var idcheck = db.tbParents.FirstOrDefault(x => x.ParentIC == log1.ID.ToString());
            var emailcheck = db.tbParents.FirstOrDefault(x => x.ParentEmail == log1.ID.ToString());

            
            if (emailcheck != null)
            {
                if (log1.ID != null)
                {
                    //var userPass = db.tbParents.Where(x => x.ParentEmail == log1.ID.ToString()).Select(x => x.Password);
                    //Materializes the query into a readable list of array objects
                   //var materializePass = userPass.ToList();
                    //Since query will only result to one , we ony have one array of result
                    //var password = materializePass[0];

                    var username = db.tbParents.Where(x => x.ParentEmail == log1.ID.ToString()).Select(x => x.ParentName);
                    var materializePass1 = username.ToList();
                    var name = materializePass1[0];

                    tbParent parentname = db.tbParents.Where(x => x.ParentEmail == log1.ID.ToString()).FirstOrDefault();


                    //Check username and password (Case Sensitive)
                    if (emailcheck.ParentEmail == log1.ID.ToString())
                    {

                        string subject = "Password recovery for Ace Education Account";
                        string message = "Here's your password: ";

                        //AceducationDummy1!
                        var senderEmail = new MailAddress("aceducationdummy1@gmail.com", "Ace Education");
                        //var receiverEmail = new MailAddress(receiver, "Receiver");
                        var password = "AceducationDummy1!";
                        var sub = subject;
                        var body = message;

                        string guid = Guid.NewGuid().ToString();

                        using (var mess = new MailMessage())
                        {
                            mess.From = new MailAddress(senderEmail.Address, "Ace Education");
                            mess.To.Add(emailcheck.ParentEmail);
                            mess.Subject = subject;
                            mess.Body = "<p>" + body + "</p>" + "<h3>" + guid + "</h3>"; //+ outbody
                            mess.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                            {
                                //smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential(senderEmail.Address, password);
                                smtp.EnableSsl = true;
                                smtp.Send(mess);
                            }
                        }

                        //Your code here to redirect
                        TempData["Message"] = "Email sent successfully";
                        return RedirectToAction("Index", "Home");
                    }
                    
                }

            }
            else
            {
                //ViewBag.ErrorMessage = "Invalid email";
                TempData["Message"] = "Invalid email";
                ViewBag.Message = TempData["Message"].ToString();
                return View();
            }
            return View();
        }


    }
}
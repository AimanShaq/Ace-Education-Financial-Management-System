using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ace_Tuition_WBL.Models;
using System.Data.SqlClient;

namespace Ace_Tuition_WBL.Controllers
{
    public class AdminController : Controller
    {
        Ace_Tuition_WBLEntities1 db = new Ace_Tuition_WBLEntities1();
       /* SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;*/
        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        /*void connectionString()
        {
            con.ConnectionString = "data source=DESKTOP-2O5J1FT; database=WPF; integrated security = SSPI;";
        }*/
        public ActionResult Login(tb_admin log)
        {
            //var user = db.tb_admin.Where(x => x.User_Id == log.User_Id && x.User_Password == log.User_Password).Count();
            var idcheck = db.tb_admin.FirstOrDefault(x => x.User_Id == log.User_Id);

            if (log.User_Id != null && idcheck != null)
            {
                var userPass = db.tb_admin.Where(x => x.User_Id == log.User_Id).Select(x => x.User_Password);
                //Materializes the query into a readable list of array objects
                var materializePass = userPass.ToList();
                //Since query will only result to one , we ony have one array of result
                var password = materializePass[0];

                //Check username and password (Case Sensitive)
                if (idcheck.User_Id == log.User_Id && log.User_Password == password)
                {
                    //Your code here to redirect
                    return RedirectToAction("AdminDashBoard");
                }
            }
            return View();
        }

        public ActionResult AdminDashBoard()
        {
            return View();
        }
    }
}
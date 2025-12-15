using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Ace_Tuition_WBL.Models;

namespace Ace_Tuition_WBL.Repository
{
    public class StudentParentRepo
    {
        //To Handle connection related activities   
        SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }
        /// <summary>  
        /// Get Multiple Table details  
        /// </summary>  
        /// <returns></returns>  
        public IEnumerable<MasterDetails> GetParentStudentDetails()
        {
            connection();
            con.Open();
            var objDetails = SqlMapper.QueryMultiple(con, "GetParentStudentDetails", commandType: CommandType.StoredProcedure);
            MasterDetails ObjMaster = new MasterDetails();

            //Assigning each Multiple tables data to specific single model class  
            ObjMaster.parentlist = objDetails.Read<tbParent>().ToList();
            ObjMaster.studentlist = objDetails.Read<tbStudent>().ToList();

            List<StudentParent> StudentParentList = new List<StudentParent>();
            //Add list of records into MasterDetails list  
            StudentParentList.Add(ObjMaster);
            con.Close();

            return StudentParentList;

        }
    }
}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ace_Tuition_WBL.Models;

namespace Ace_Tuition_WBL.ViewModel
{
    public class SubjectModel
    {
        public List<tbSubject> Subjectlist { get; set; }
        public tbStudent Studentget { get; set; }

        public List<tbRegistration> reglist { get; set; }
    }

    public class SubjectReg
    {
        public IEnumerable<tbRegistration> RegistrationList { get; set; }
        public IEnumerable<tbStudent> StudentgetList { get; set; }
    }

}
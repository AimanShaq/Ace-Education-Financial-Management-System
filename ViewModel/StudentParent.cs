using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ace_Tuition_WBL.Models;

namespace Ace_Tuition_WBL.ViewModel
{
    public class StudentParent
    {
        public tbStudent studentdetail { get; set; }
        public tbParent parentdetail { get; set; }
        public IEnumerable<tbParent> parentlist { get; set; }
        public IEnumerable<tbStudent> studentlist { get; set; }

    }

    
}
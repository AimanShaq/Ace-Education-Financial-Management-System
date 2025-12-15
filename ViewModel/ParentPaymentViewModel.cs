using Ace_Tuition_WBL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ace_Tuition_WBL.ViewModel
{
    public class ParentPaymentViewModel
    {
        public int ReceiptID { get; set; }
        public int ParentID { get; set; }
        public float ReceiptTotal { get; set; }
        public float ReceiptAmount { get; set; }
        public int StudentID { get; set; }
        public DateTime ReceiptDate { get; set; }
        public float ReceiptDiscount { get; set; }
        public string ReceiptDesc { get; set; }
        public HttpPostedFileBase ReceiptProof { get; set; }
        public int earlyReg { get; set; }
        public int firstReg { get; set; }
        public int ReceiptStatus { get; set; }
        public IEnumerable<SelectListItem> ReceiptList { get; set; }

        public tbReceipt receiptdetails { get; set; }
        public tbStudent studentdetails { get; set; }
    }
}
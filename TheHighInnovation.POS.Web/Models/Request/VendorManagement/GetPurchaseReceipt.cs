using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Web.Model.Request.VendorManagement
{
    public class GetPurchaseReceipt
    {
        public int Id { get; set; }
        public string VendorName { get; set; }  
        public int VendorId { get; set;}
        public string IssueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentStatus { get; set; }
    }
  
}

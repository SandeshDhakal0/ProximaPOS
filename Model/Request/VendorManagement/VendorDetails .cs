using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.WEB.Model.Request.VendorManagement
{
    public class VendorDetails
    {
        public string p_vendor_name { get; set; }
        public int p_vendor_id { get; set; }
        public string p_issue_date { get; set; }
        public string p_invoice_no { get; set; }
        public string p_payment_status { get; set; }
    }
}

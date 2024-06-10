using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Request.PurchaseReceipt
{
    public class PurchaseReceiptDetails
    {
        public string p_vendor_name {  get; set; }
        public string p_pan_vat { get; set; }
        public int p_vendor_id { get; set; }
        public string p_issue_date_ad { get; set; }
        public string p_issue_date_bs { get; set; }
        public string p_branch { get; set; }
        public string p_invoice_no { get; set; }
        public string p_due_date_ad { get; set; }   
        public string p_due_date_bs { get; set; }
        public string p_receipt_type { get; set; }
        public string p_bill_no { get; set; }
        public string p_mop { get; set; }
        public string? p_remarks { get; set; } 
        public string? p_bill_prepared_by { get; set; }
        public string? p_amount_in_words { get; set; }
        public decimal? p_gross_amount { get; set; }
        public decimal? p_product_discount { get; set; }
        public decimal? p_discount { get; set; } = 0;
        public decimal? p_taxable_amount { get; set; }
        public decimal? p_vat { get; set; }
        public decimal? p_rounded_off { get; set; }
        public decimal? p_net_amount { get; set; }
        public decimal? p_paid_amount { get; set; }
        public decimal? p_remaining_amount { get; set; }
        public int p_company_id { get; set; }
        public int p_year_bs { get; set; }
        public int p_year_ad { get; set; }
        public int p_month_bs { get; set; }
        public int p_month_ad { get; set; }
    }
}

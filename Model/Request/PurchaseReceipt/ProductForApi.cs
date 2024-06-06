using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.WEB.Model.Request.PurchaseReceipt
{
    public class ProductForApi
    {
        public string p_product_name { get; set; }
        public int p_qty { get; set; }
        public string p_unit { get; set; }
        public decimal? p_disc { get; set; }
        public decimal p_rate { get; set; }
        public decimal p_amount { get; set; }
        public int p_product_receipt_id { get; set; }
        public int p_product_id { get; set; }
        public string p_uom { get; set; }
        public decimal p_disc_percent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.WEB.Model.Request.VendorManagement
{
    public class VendorWiseProductById
    {        
        public string VendorName { get; set; }
        public string Pan_VatNo { get; set; }
        public string IssueDateAd { get; set; }
        public string IssueDateBs { get; set; }
        public string ProductDetails { get; set; }

    }

    public class VendorWiseProductByIdMap
    {
        public string VendorName { get; set; }
        public string Pan_VatNo { get; set; }
        public string IssueDateAd { get; set; }
        public string IssueDateBs { get; set; }
        public VendorProductDetails[] ProductDetails { get; set; }
    }

    public class VendorProductDetails
    {
        public string Unit { get; set; }
        public decimal Rate { get; set; }
        public int Qty { get; set; }
        public decimal Disc { get; set; }
        public decimal DiscPercent { get; set; }
        public string ProductName { get; set; }

    }
}

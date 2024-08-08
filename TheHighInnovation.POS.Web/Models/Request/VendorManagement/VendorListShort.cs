using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Web.Model.Request.VendorManagement
{
    public class VendorListShort
    {
        public int Id { get; set; } = 0;
        public string? CompanyName { get; set; }
        public string? PanVat { get; set; }

    }

    public class ProductListShort
    {
        public int Id { get; set; } 
        public string ProductName { get; set; } 
        public string Barcode { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalesPrice { get; set; }
        public int CategoryId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Request.VendorManagement.ProductsDTO
{
    public class ProductListUpdate
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string DefaultUnit { get; set; }
        public decimal SalesPrice { get; set; }
        public string SKU { get; set; }
        public decimal UnitSize { get; set; }
        public bool Taxable { get; set; }
        public bool IsActive { get; set; }
    }
}

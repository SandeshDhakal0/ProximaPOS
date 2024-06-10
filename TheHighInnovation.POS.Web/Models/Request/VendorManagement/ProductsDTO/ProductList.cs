using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Request.VendorManagement.ProductsDTO
{
    public class ProductList
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string Unit { get; set; }
        public decimal SalesPrice { get; set; }
        public string CategoryName { get; set; }
        public bool Taxable { get; set; }
        public bool IsActive { get; set; }
    }
   

    public class ProductListDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string Unit { get; set; }
        public decimal SalesPrice { get; set; }
        public string CategoryName { get; set; }

        public bool Taxable { get; set; }
        public bool IsActive { get; set; }
    }
}

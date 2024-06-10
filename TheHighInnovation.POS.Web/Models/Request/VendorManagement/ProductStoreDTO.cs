using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Request.VendorManagement
{
    public class ProductStoreDTO
    {
        public string p_product_name { get; set; }
        public int p_category_id { get; set; }
        public string? p_barcode_code { get; set; }
        public string? p_description { get; set; }
     //   public int p_qty { get; set; }
        public string? p_unit { get; set; }
       // public decimal p_cost_price { get; set; }
        public decimal p_sales_price { get; set; }
        public string? p_sku { get; set; }
        public bool p_taxable { get; set; }
        public bool p_is_active { get; set; }
        public int p_company_id { get; set; }
        public string? p_default_size { get; set; }
        public string? p_unit_size { get; set; }

    }
    public class ProductFormList
    {
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string? BarcodeCode { get; set; }
        public string? Description { get; set; }
        public int Qty { get; set; }
        public string? Unit { get; set; }
        public string? SKU { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool Taxable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? DefaultUnit { get; set; }
        public string? UnitSize { get; set; }
    }
    
}

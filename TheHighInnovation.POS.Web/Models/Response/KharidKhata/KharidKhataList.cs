using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Response.KharidKhata
{
    public class KharidKhataList
    {
        public string InvoiceNo { get; set; }
        public string? IssueDateBs { get; set; }
        public string? VendorName { get; set; }
        public string? VendorPanVat { get; set; }
        public string? ProductName { get; set; }
        public string? Unit {  get; set; }
        public int Qty {  get; set; }
        public decimal Rate { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }

    }
}

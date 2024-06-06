using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.WEB.Model.Response
{
    public class ProductScannedResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal SalesPrice { get; set; }
    }
}

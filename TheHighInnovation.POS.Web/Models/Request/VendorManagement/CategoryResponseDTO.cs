using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Request.VendorManagement
{
    public class CategoryResponseDTO
    {
        public int Id { get; set; } 
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        
    }

    public class CategoryList
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDescription { get; set; }
        public bool IsActive { get; set; }
    }

}

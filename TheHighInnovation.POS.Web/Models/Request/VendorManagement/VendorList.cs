﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Web.Model.Request.VendorManagement
{
    public class VendorList
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Pan_VatNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNo { get; set; }
        public string? VendorCode { get; set; }
        public bool IsActive { get; set; }
    }

   

}

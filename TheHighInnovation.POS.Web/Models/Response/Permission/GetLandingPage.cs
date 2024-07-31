using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Web.Model.Response.Permission
{
    public class GetLandingPage
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Icon { get; set; }
        public string Action { get; set; }

    }
}

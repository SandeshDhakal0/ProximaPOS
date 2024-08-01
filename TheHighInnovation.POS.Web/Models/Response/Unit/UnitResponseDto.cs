using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Web.Model.Response.Unit
{
	internal class UnitResponseDto
	{
		public int p_id { get; set; }
		public string p_measure { get; set; }
	}
    public class UnitList
    {
        public int Id { get; set; }
        public string Measure { get; set; }

    }

    public class UnitListPost
    {
        public int Id { get; set; }
        public string Measure { get; set; }

    }

    public class UnitById
    {
       
        public string Measure { get; set; }
    }
}

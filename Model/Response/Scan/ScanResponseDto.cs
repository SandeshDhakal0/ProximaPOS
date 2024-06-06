using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.WEB.Model.Response.Scan
{
	public class ScanResponseDto
	{
		public int p_id { get; set; }

		public string p_barcode { get; set; } = "string";

		public string p_title { get; set; }

		public int p_unit { get; set; }

		public int p_companyid { get; set; }

		public string p_imageurl { get; set; }

		public decimal p_salesprice { get; set; }

		public decimal p_costprice { get; set; }
	}
}

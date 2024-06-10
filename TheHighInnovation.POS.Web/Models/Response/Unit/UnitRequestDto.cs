using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace TheHighInnovation.POS.Model.Response.Unit
{
	internal class UnitRequestDto
	{
		public int Id { get; set; }

		public string Measure { get; set; }

		public string CreatedAt { get; set; }

	}
}

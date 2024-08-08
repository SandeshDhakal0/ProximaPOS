using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace TheHighInnovation.POS.Web.Model.Response.Category;

public class CategoryRequestDto
{
    public int Id { get; set; }
    
    public string Title { get; set; }

    public string Description { get; set; }

    public int? CompanyId { get; set; }
    
    public int? ParentCategoryId { get; set; }
    
    public string? ImageURL { get; set; }
}
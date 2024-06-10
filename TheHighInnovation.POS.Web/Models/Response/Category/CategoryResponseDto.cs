namespace TheHighInnovation.POS.Model.Response.Category;

public class CategoryResponseDto
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public int? CompanyId { get; set; }
    
    public string CompanyName { get; set; }
    
    public int? ParentCategoryId { get; set; }
    
    public string? ParentCategoryName { get; set; }
    
    public string ImageUrl { get; set; }
}
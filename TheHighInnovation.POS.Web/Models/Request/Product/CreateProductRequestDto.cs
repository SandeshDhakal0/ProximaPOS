using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Product;

public class CreateProductRequestDto
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public int Unit { get; set; }
    
    public int? CategoryId { get; set; }    
    
    public int? CompanyId { get; set; }
    
    public decimal ? SalesPrice { get; set; }
    
    public decimal? CostPrice { get; set; }
    
    public string? ImageURL { get; set; }
}
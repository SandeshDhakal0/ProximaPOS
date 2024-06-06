namespace TheHighInnovation.POS.WEB.Model.Response.Product;

public class ProductResponseDto
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public int Unit { get; set; }
    
    public int? CategoryId { get; set; }
    
    public string? CategoryTitle { get; set; }
    
    public int? CompanyId { get; set; }
    
    public string? CompanyName { get; set; }
    
    public string ProductImageUrl { get; set; }
    
    public decimal? RemainingQuantity { get; set; }
    
    public decimal? SalesPrice { get; set; }
    
    public decimal? CostPrice { get; set; }

    public int? OrganizationId { get; set; }

    public string? OrganizationName { get; set; }


}


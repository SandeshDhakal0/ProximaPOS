namespace TheHighInnovation.POS.Web.Models.Dashboard;

public class TopSelling
{
    public string? productname { get; set; }
    
    public decimal? quantity { get; set; }
    
    public decimal? total_amount { get; set; }
}

public class TopSellingDto(TopSelling topSelling)
{
    public string ProductName { get; set; } = topSelling.productname ?? "";
    
    public decimal Quantity { get; set; } = topSelling.quantity ?? 0;
    
    public decimal TotalAmount { get; set; } = topSelling.total_amount ?? 0;
}
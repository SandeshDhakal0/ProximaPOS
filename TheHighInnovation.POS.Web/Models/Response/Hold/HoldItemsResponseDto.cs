namespace TheHighInnovation.POS.Model.Response.Hold;

public class Hold
{
    public string HoldType { get; set; }

    public int HoldId { get; set; }

    public bool IsActive { get; set; }

    public decimal? DiscountAmount { get; set; }
    
    public List<ProductDetail> ProductDetails { get; set; }
}


public class ProductDetail
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal ProductAmount { get; set; }
    
    public decimal DiscountAmount { get; set; }
}

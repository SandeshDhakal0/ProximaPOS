namespace TheHighInnovation.POS.Web.Model.Response.Report;

public class VoidDetailsResponseDto
{
    public int Id { get; set; }

    public string Type { get; set; }

    public List<ProductsDetails> ProductsDetails { get; set; }
    
    public DateTime Date { get; set; }
    
    public TimeSpan Time { get; set; }
    
    public int CashierId { get; set; }
    
    public string CashierName { get; set; }
    
    public decimal VoidDiscount { get; set; }

    public decimal VoidTaxAmount { get; set; }

    public decimal VoidTotalAmount { get; set; }
}

public class ProductsDetails
{
    public int Id { get; set; }
    
    public string ProductName { get; set; }
    
    public decimal Quantity { get; set; }
    
    public decimal ProductAmount { get; set; }
    
    public decimal DiscountAmount { get; set; }
}
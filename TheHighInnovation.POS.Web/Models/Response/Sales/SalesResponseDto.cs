namespace TheHighInnovation.POS.Web.Model.Response.Sales;

public class SalesResponseDto
{
    public int Id { get; set; }
    
    public decimal TotalAmountAfterDiscount { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public int CustomerId { get; set; }
    
    public int CashierId { get; set; }
    
    public string TransactionOption { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CashierName { get; set; }
    public SalesProduct[] ProductsDetail { get; set; }
}

public class SalesProduct
{
    public int Id { get; set; }
    
    public string? ProductName { get; set; }
    
    public decimal Quantity { get; set; }
    
    public decimal ProductAmount { get; set; }
    
    public decimal DiscountAmount { get; set; }

}

public class SalesProductDto
{
    public string? CustomerFullName { get; set; }
    public string? CashierName { get; set; }
    public decimal TotalAmountAfterDiscount { get; set; }
}
namespace TheHighInnovation.POS.Web.Model.Response.CloseTill;

public class CloseTillResponseDto
{
    public string TransactionOption { get; set; }
    
    public decimal TotalSales { get; set; }
    
    public decimal TotalAfterDiscount { get; set; }
    
    public decimal TotalDiscountGiven { get; set; }
    
    public decimal TotalSafeDrop { get; set; }
}
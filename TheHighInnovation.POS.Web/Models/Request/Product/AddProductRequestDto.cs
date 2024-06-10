namespace TheHighInnovation.POS.Model.Request.Product;

public class AddProductRequestDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1;
                   
    public decimal UnitPrice { get; set; } = 1;

    public decimal Discount { get; set; } = 0;
    
    public decimal TotalPrice { get; set; } = 1;
}
namespace TheHighInnovation.POS.Web.Model.Response.BikriKhata;

public class BikriKhataDetailsDto
{
    public string BijakNumber { get; set; }
    
    public DateTime MitiDate { get; set; }
    
    public string NepaliDates { get; set; }
    
    public string CustomerName { get; set; }
    
    public string? LekhaNumber { get; set; }
    
    public string ProductName { get; set; }
    
    public decimal Quantity { get; set; }
    
    public decimal ProductAmount { get; set; }
    
    public decimal VatAmount { get; set; }
}
namespace TheHighInnovation.POS.Web.Model.Response.Report;

public class SafeDropResponseDto
{
    public int Id { get; set; }
    
    public AmountDetails Amount { get; set; }
    
    public DateTime Date { get; set; }
    
    public TimeSpan Time { get; set; }
    
    public int CashierId { get; set; }
    
    public string CashierName { get; set; }
    
    public int TotalAmount { get; set; }
}

public class AmountDetails
{
    public int Rs1 { get; set; }
    
    public int Rs2 { get; set; }
    
    public int Rs5 { get; set; }
    
    public int Rs10 { get; set; }
    
    public int Rs20 { get; set; }
    
    public int Rs50 { get; set; }
    
    public int Rs100 { get; set; }
    
    public int Rs500 { get; set; }
    
    public int Rs1000 { get; set; }
}

public class CombinedAmountDetails
{
    public int Rs1 { get; set; }
    
    public int Rs2 { get; set; }
    
    public int Rs5 { get; set; }
    
    public int Rs10 { get; set; }
    
    public int Rs20 { get; set; }
    
    public int Rs50 { get; set; }
    
    public int Rs100 { get; set; }
    
    public int Rs500 { get; set; }
    
    public int Rs1000 { get; set; }
}
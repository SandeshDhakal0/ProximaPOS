namespace TheHighInnovation.POS.WEB.Model.Request.SafeDrop;

public class SafeDropRequestDto
{
    public int p_cashierid { get; set; }
    
    public decimal p_totalamount { get; set; }
    
    public CashDetails p_amount { get; set; }
}

public class CashDetails
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
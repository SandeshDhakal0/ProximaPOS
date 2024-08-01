namespace TheHighInnovation.POS.Web.Model.Response.Report;

public class PettyCashResponseDto
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Particulars { get; set; }
    
    public string FullName { get; set; }
    
    public string Approver { get; set; }
    
    public string Remarks { get; set; }
    
    public int CashierId { get; set; }
    
    public int TotalAmount { get; set; }
}
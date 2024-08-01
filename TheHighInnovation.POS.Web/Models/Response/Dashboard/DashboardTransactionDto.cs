namespace TheHighInnovation.POS.Web.Model.Response.Dashboard;

public class DashboardTransactionDto
{
    public double CashTotal { get; set; }
    
    public double CardTotal { get; set; }

    public double QrTotal { get; set; }
}
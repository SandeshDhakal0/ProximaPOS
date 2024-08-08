namespace TheHighInnovation.POS.Web.Model.Request.Report;

public class ReportSearchDto
{
    public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

    public DateTime EndDate { get; set; } = DateTime.Now;

    public int PageSize { get; set; } = 5;
    
    public int PageNumber { get; set; }
}
namespace TheHighInnovation.POS.Web.Models.Dashboard;

public class MoneySales
{
    public string? x_axis_legend { get; set; }
    
    public decimal? y_axis_amount { get; set; }
}

public class MoneySalesDto(MoneySales moneySales)
{
    public string Period { get; set; } = moneySales.x_axis_legend ?? "";
    
    public decimal Amount { get; set; } = moneySales.y_axis_amount ?? 0;
}
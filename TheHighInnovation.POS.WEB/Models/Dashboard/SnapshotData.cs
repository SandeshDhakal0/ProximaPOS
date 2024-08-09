namespace TheHighInnovation.POS.Web.Models.Dashboard;

public class SnapshotData
{
    public string? most_working_day { get; set; }
    
    public decimal? avg_discount { get; set; }
    
    public decimal? avg_total_amount_after_discount { get; set; }
}

public class SnapshotDto(SnapshotData snapshot)
{
    public string MostWorkingDay { get; set; } = snapshot.most_working_day ?? "";
    
    public decimal AvgDiscount { get; set; } = snapshot.avg_discount ?? 0;
    
    public decimal AvgTotalAmountAfterDiscount { get; set; } = snapshot.avg_total_amount_after_discount ?? 0;
}
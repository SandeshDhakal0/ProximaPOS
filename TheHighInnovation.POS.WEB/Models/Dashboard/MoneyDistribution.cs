namespace TheHighInnovation.POS.Web.Models.Dashboard;

public class MoneyDistribution
{
    public string? transactionoption { get; set; }
    
    public double? total_amount { get; set; }
    
    public double? percentage { get; set; }
}

public class MoneyDistributionDto(MoneyDistribution moneyDistribution)
{
    public string TransactionOption { get; set; } = moneyDistribution.transactionoption ?? "";
    
    public decimal TotalAmount { get; set; } = (decimal) (moneyDistribution.total_amount ?? 0);
    
    public decimal Percentage { get; set; } = (decimal)(moneyDistribution.percentage ?? 0);
}
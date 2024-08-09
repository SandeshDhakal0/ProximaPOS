namespace TheHighInnovation.POS.Web.Models.Dashboard;

public class DashboardRecords
{
    public double? total_revenue { get; set; }
    public double? total_petty_cash { get; set; }
    public double? total_sales { get; set; }
    public double? petty_cash { get; set; }
    public double? bar_sales { get; set; }
    public double? occupancy_rate { get; set; }
    public double? total_inventory_rupees { get; set; }
    public double? total_inventory_units { get; set; }
    public double? restro_overview_table_sales { get; set; }
    public double? restro_overview_online_sales { get; set; }
    public double? order_status_total { get; set; }
    public double? order_status_return { get; set; }
    public double? customer_status_order { get; set; }
    public double? customer_status_online { get; set; }
}

public class DashboardRecordsDto(DashboardRecords dashboardRecords)
{
    public double TotalRevenue { get; set; } = dashboardRecords.total_revenue ?? 0;
    
    public double TotalPettyCash { get; set; } = dashboardRecords.total_petty_cash ?? 0;
    
    public double TotalSales { get; set; } = dashboardRecords.total_sales ?? 0;
    
    public double PettyCash { get; set; } = dashboardRecords.petty_cash ?? 0;
    
    public double BarSales { get; set; } = dashboardRecords.bar_sales ?? 0;
    
    public double OccupancyRate { get; set; } = dashboardRecords.occupancy_rate ?? 0;
    
    public double TotalInventoryRupees { get; set; } = dashboardRecords.total_inventory_rupees ?? 0;
    
    public double TotalInventoryUnits { get; set; } = dashboardRecords.total_inventory_units ?? 0;
    
    public double RestroOverviewTableSales { get; set; } = dashboardRecords.restro_overview_table_sales ?? 0;
    
    public double RestroOverviewOnlineSales { get; set; } = dashboardRecords.restro_overview_online_sales ?? 0;
    
    public double OrderStatusTotal { get; set; } = dashboardRecords.order_status_total ?? 0;
    
    public double OrderStatusReturn { get; set; } = dashboardRecords.order_status_return ?? 0;
    
    public double CustomerStatusOrder { get; set; } = dashboardRecords.customer_status_order ?? 0;
    
    public double CustomerStatusOnline { get; set; } = dashboardRecords.customer_status_online ?? 0;
}
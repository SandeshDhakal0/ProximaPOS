using ApexCharts;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Models;
using TheHighInnovation.POS.Web.Models.Dashboard;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Dashboard
{
    [CascadingParameter] private GlobalState GlobalState { get; set; }

    private const string Daily = "daily";
    private const string Weekly = "weekly";
    private const string Monthly = "monthly";
    private const string Yearly = "yearly";

    private const string Sales = "Sales";
    private const string Snapshots = "Snapshots";
    private const string TopSellingRecord = "TopSellingRecord";
    private const string MoneyDistribution = "MoneyDistribution";

    private bool IsSalesChartRendered { get; set; } = true;
    private bool IsMoneyDistributionChartRendered { get; set; } = true;
    
    #region Dashboard Records
    private DashboardRecordsDto? DashboardRecords { get; set; }

    private async Task GetDashboardRecords()
    {
        var dashboardRecords = await BaseService.GetAsync<Derived<DashboardRecords>>("dashboard/get-admin-dashboard-data");

        if(dashboardRecords?.Result != null) DashboardRecords = new DashboardRecordsDto(dashboardRecords.Result);
    }
    #endregion

    #region Sales & Profit
    private List<MoneySalesDto>? SalesRecords { get; set; }
    
    private async Task GetSalesAndProfitRecords(string? filter = "yearly")
    {
        var salesAndProfitRecords = await BaseService.GetAsync<Derived<List<MoneySales>>>($"dashboard/get-money-sales-data/{filter}");

        if (salesAndProfitRecords?.Result != null) 
            SalesRecords = salesAndProfitRecords.Result.ConvertAll(x => new MoneySalesDto(x)).ToList();
    }
    #endregion

    #region Money Distribution
    public List<MoneyDistributionDto>? MoneyDistributionRecords { get; set; }

    private async Task GetMoneyDistributionRecords(string? filter = "daily")
    {
        var moneyDistributionRecords = await BaseService.GetAsync<Derived<List<MoneyDistribution>>>($"dashboard/get-money-distribution-data/{filter}");

        if (moneyDistributionRecords?.Result != null) 
            MoneyDistributionRecords = moneyDistributionRecords.Result
                .Where(x => x.transactionoption != null).ToList()
                .ConvertAll(x => new MoneyDistributionDto(x)).ToList();
    }
    #endregion

    #region Top Selling Records
    private List<TopSellingDto>? TopSellingRecords { get; set; }

    private async Task GetTopSellingRecords(string? filter = "daily")
    {
        var topSellingRecords = await BaseService.GetAsync<Derived<List<TopSelling>>>($"dashboard/get-top-selling-data/{filter}");

        if (topSellingRecords?.Result != null) 
            TopSellingRecords = topSellingRecords.Result
                .ConvertAll(x => new TopSellingDto(x))
                .OrderByDescending(z => z.TotalAmount).Take(5).ToList();
    }
    #endregion

    #region Snapshots
    private SnapshotDto? SnapshotRecords { get; set; }
    
    private async Task GetSnapshotRecords(string? filter = "weekly")
    {
        var snapshotRecords = await BaseService.GetAsync<Derived<SnapshotData>>($"dashboard/get-snapshot-data/{filter}");

        if (snapshotRecords?.Result != null) 
            SnapshotRecords = new SnapshotDto(snapshotRecords.Result);
    }
    #endregion

    #region Filter
    private async Task HandleFilterChange(ChangeEventArgs e, string component)
    {
        var filterValue = e.Value?.ToString();
        
        switch (component)
        {
            case Sales:
                Render();
                await GetSalesAndProfitRecords(filterValue);
                Render();
                break;
            case MoneyDistribution:
                Render();
                await GetMoneyDistributionRecords(filterValue);
                Render();
                break;
            case TopSellingRecord:
                await GetTopSellingRecords(filterValue);
                break;
            case Snapshots:
                await GetSnapshotRecords(filterValue);
                break;
        }
    }
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        await GetDashboardRecords();

        await GetSalesAndProfitRecords();

        await GetMoneyDistributionRecords();

        await GetTopSellingRecords();

        await GetSnapshotRecords();
    }

    // private DashboardTransactionDto DashboardTransactionDetails { get; set; } = new();
    //
    // protected override async Task OnInitializedAsync()
    // {
    //     GlobalState = await BaseService.GetGlobalState();
    //     
    //     if (GlobalState.CompanyId is not null)
    //     {
    //         var parameters = new Dictionary<string, string>
    //         {
    //             { "companyId", GlobalState.CompanyId!.ToString()! },
    //         };
    //     
    //         var result = await BaseService.GetAsync<Derived<List<DashboardTransactionDetails>>>("dashboard/get-sales-records", parameters);
    //     
    //         DashboardTransactionDetails = result?.Result.FirstOrDefault() ?? new();
    //     }
    // }

    private void Render()
    {
        IsSalesChartRendered = !IsSalesChartRendered;
        IsMoneyDistributionChartRendered = !IsMoneyDistributionChartRendered;
    }
}
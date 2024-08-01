using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Components.Web;
using TheHighInnovation.POS.Web.Model;
using TheHighInnovation.POS.Web.Model.Request.Filter;
using TheHighInnovation.POS.Web.Model.Response.Company;
using TheHighInnovation.POS.Web.Model.Response.Sales;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Sale
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }

    private List<SalesResponseDto> _sales { get; set; } = [];

    private SalesProductDto _salesRecord { get; set; }

    private List<SalesProduct> _salesDetails { get; set; } = [];

    private FilterRequestDto Filter = new();

    private List<CompanyResponseDto> _companies { get; set; } = new();

    private PagerDto _pagerDto { get; set; } = new();

    private bool OpenSalesRecordDialog { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _jsRuntime.InvokeVoidAsync("leapfrogdatepicker");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (_globalState.OrganizationId != null)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", _globalState.OrganizationId.ToString()! },
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);

            await HandleFilter();

            _companies = companies!.Result;
        }
    }

    private async Task OnOptionSelection(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();

        Filter.TransactionOption = value;
    }

    private async Task OnCompanySelection(ChangeEventArgs e)
    {
        if (e.Value == null) return;

        Filter.CompanyId = Int32.Parse(e.Value.ToString());
    }



    private async Task HandleFilter()
    {

        if (_globalState.OrganizationId is not null)
        {
            if (_globalState.CompanyId is not null)
            {
                string companyId;
                string transactionOption;
                Filter.PageSize = 10;
                if (Filter.CompanyId.ToString() == "0")
                {
                    companyId = _globalState.CompanyId.ToString()!;
                    transactionOption = "All";

                }
                else
                {
                    companyId = Filter.CompanyId.ToString()!;
                    transactionOption = Filter.TransactionOption;
                }

                ChangeEventArgs args = new ChangeEventArgs();
                args.Value = transactionOption;
                await OnOptionSelection(args);

                ChangeEventArgs brgs = new ChangeEventArgs();
                brgs.Value = companyId;
                await OnCompanySelection(brgs);

                var initialParameters = new Dictionary<string, string>
                {
                { "company_id", companyId },
                { "start_date", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd")},
                { "end_date", DateTime.Now.ToString("yyyy-MM-dd")},
                { "transaction_option", transactionOption },
                { "page", "1" },
                { "page_size", Filter.PageSize.ToString() },
            };
                var initialSales = await BaseService.GetAsync<Derived<List<SalesResponseDto>>>("sales/get-sales-records", initialParameters);


                _pagerDto = new PagerDto(initialSales.TotalCount ?? 1, 1, 10);

                _sales = initialSales.Result ?? [];

                Filter.IsInitialized = true;
            }

        }
    }

    private async Task OpenSalesRecord(int salesId)
    {
        var salesRecord = _sales.FirstOrDefault(x => x.Id == salesId);

        if (salesRecord is null) return;

        _salesDetails = salesRecord.ProductsDetail.ToList();

        _salesRecord = new SalesProductDto
        {
            CustomerFullName = salesRecord.CustomerFullName,
            CashierName = salesRecord.CashierName,
            TotalAmountAfterDiscount = salesRecord.TotalAmountAfterDiscount,
        };

        OpenSalesRecordDialog = true;
    }


    private string? x = "";

    private void OnStartDateInput(ChangeEventArgs e)
    {
        x = e.Value?.ToString();
    }

    private void OnStartDateBlur(FocusEventArgs e)
    {
        if (!string.IsNullOrEmpty(x))
        {
            OnStartDateSelection(x);
        }
    }

    private void OnStartDateSelection(string startDate)
    {
        var z = startDate;
        // Your logic for handling the start date selection
    }
    private async Task CloseSalesRecord()
    {
        OpenSalesRecordDialog = false;
    }

    private async Task HandlePaginationChange(ChangeEventArgs e)
    {
        if (e.Value == null) return;

        var pageSize = int.Parse(e.Value.ToString()!);

        Filter.PageSize = pageSize;

        await OnPagination(1);
    }

    private async Task OnPagination(int pageNumber)
    {
        var pageSize = Filter.PageSize;

        var parameters = new Dictionary<string, string>
        {
            { "company_id", Filter.CompanyId.ToString()! },
            { "start_date", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd")},
            { "end_date", DateTime.Now.ToString("yyyy-MM-dd")},
            { "transaction_option", Filter.TransactionOption},
            { "page", pageNumber.ToString() },
            { "page_size", pageSize.ToString() },
        };

        var sales = await BaseService.GetAsync<Derived<List<SalesResponseDto>>>("sales/get-sales-records", parameters);

        _pagerDto = new PagerDto(sales.TotalCount ?? 1, pageNumber, pageSize);

        _sales = sales.Result ?? [];
    }
}
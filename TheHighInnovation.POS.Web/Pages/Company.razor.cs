using Microsoft.AspNetCore.Components;
using System.Text.Json;
using TheHighInnovation.POS.Model;
using TheHighInnovation.POS.Model.Request.Company;
using TheHighInnovation.POS.Model.Request.Filter;
using TheHighInnovation.POS.Model.Response.Company;
using TheHighInnovation.POS.Model.Response.Organization;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Company
{
	[CascadingParameter]
	private GlobalState _globalState { get; set; }

	private List<CompanyResponseDto> _companies { get; set; }

    private List<OrganizationResponseDto> _organizations { get; set; }
    
    private bool _showUpsertCompanyDialog { get; set; }
    
    private bool _showDeleteCompanyDialog { get; set; }

    private CompanyRequestDto _companyModel { get; set; } = new();
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertCompanyErrorMessage { get; set; }
    
    private string _deleteCompanyErrorMessage { get; set; }

    private FilterRequestDto Filter = new();

    private PagerDto _pagerDto { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
	    Filter.PageSize = 5;

		if (_globalState.OrganizationId != null)
		{
			var parameters = new Dictionary<string, string>
		    {
			    { "organizationId", _globalState.OrganizationId.ToString()! },
			    { "pageNumber", "1" },
			    { "pageSize", Filter.PageSize.ToString() },
		    };

			var organizations = await BaseService.GetAsync<Model.Response.Base.Derived<OrganizationResponseDto>>("organization", parameters);

			_organizations = new()
            {
				organizations.Result
			};

			var companies = await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);

			_pagerDto = new PagerDto(companies.TotalCount ?? 1, 1, 5);

			_companies = companies?.Result ?? [];

            _companyModel.OrganizationId = _globalState.OrganizationId ?? 1;
		}
		else
        {
	        var parameters = new Dictionary<string, string>
	        {
		        { "pageNumber", "1" },
		        { "pageSize", Filter.PageSize.ToString() },
	        };

	        var organizations = await BaseService.GetAsync<Model.Response.Base.Derived<List<OrganizationResponseDto>>>("organization");

	        _organizations = organizations.Result;
	        
			var companies = await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);

			_pagerDto = new PagerDto(companies.TotalCount ?? 1, 1, 10);

			_companies = companies?.Result ?? [];
		}
    }

    private async Task HandlePaginationChange(ChangeEventArgs e)
    {
	    if(e.Value == null) return;

	    var pageSize = int.Parse(e.Value.ToString()!);

	    Filter.PageSize = pageSize;

	    await OnPagination(1);
    }

    private async Task OnPagination(int pageNumber)
    {
	    var pageSize = Filter.PageSize;
	    
	    if (_globalState.OrganizationId != null)
	    {
		    var parameters = new Dictionary<string, string>
		    {
			    { "organizationId", _globalState.OrganizationId.ToString()! },
			    { "pageNumber", pageNumber.ToString() },
			    { "pageSize", pageSize.ToString() },
		    };

		    var companies = await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);

		    _pagerDto = new PagerDto(companies.TotalCount ?? 1, pageNumber, pageSize);

		    _companies = companies?.Result ?? [];

		    _companyModel.OrganizationId = _globalState.OrganizationId ?? 1;
	    }
	    else
	    {
		    var parameters = new Dictionary<string, string>
		    {
			    { "pageNumber", pageNumber.ToString() },
			    { "pageSize", pageSize.ToString() },
		    };
		    
		    var companies = await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);
		    
		    _pagerDto = new PagerDto(companies.TotalCount ?? 1, pageNumber, pageSize);

		    _companies = companies?.Result ?? [];
	    }
    }
    
    private async Task OpenUpsertCompanyDialog(int? companyId = null)
    {
        _dialogTitle = "Add a new company";

        _dialogOkLabel = "Add";

        _upsertCompanyErrorMessage = "";

        _showUpsertCompanyDialog = true;

        if (companyId.HasValue)
        {
            var parameters = new Dictionary<string, string>
            {
                { "companyId", companyId.Value.ToString() },
            };

            var result = (await BaseService.GetAsync<Model.Response.Base.Derived<CompanyResponseDto>>("company", parameters))?.Result;

            _companyModel = new CompanyRequestDto()
            {
                Id = result!.Id,
                Name = result.Name,
                Location = result.Location,
                OrganizationId = result.OrganizationId
            };
        }
        else
        {
            _companyModel = new CompanyRequestDto();
        }
    }

    private async Task OnUpsertCompanyDialogClose(bool isClosed)
    {
	    try
	    {
		    if (isClosed)
		    {
			    _showUpsertCompanyDialog = false;
		    }
		    else
		    {
			    if (string.IsNullOrEmpty(_companyModel.Name) || _companyModel.OrganizationId == 0 || string.IsNullOrEmpty(_companyModel.Location))
			    {
				    _upsertCompanyErrorMessage = "Please fill in all the details.";
            
				    return;
			    }
	        
			    var jsonRequest = JsonSerializer.Serialize(_companyModel);

			    var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

			    await BaseService.PostAsync<Model.Response.Base.Derived<object>>("company", content);

			    _showUpsertCompanyDialog = false;
            
			    await OnInitializedAsync();
		    }
	    }
	    catch (Exception e)
	    {
		    _upsertCompanyErrorMessage = e.Message;
	    }
        
    }
}
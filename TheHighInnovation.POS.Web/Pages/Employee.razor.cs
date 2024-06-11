using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Model;
using TheHighInnovation.POS.Model.Request.Employee;
using TheHighInnovation.POS.Model.Request.Filter;
using TheHighInnovation.POS.Model.Response.Company;
using TheHighInnovation.POS.Model.Response.Employee;
using TheHighInnovation.POS.Model.Response.Organization;
using TheHighInnovation.POS.Model.Response.Role;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Employee
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }
    
    private List<EmployeeResponseDto> _employees { get; set; }
    
    private bool _showUpsertEmployeeDialog { get; set; }
    
    private bool _showDeleteEmployeeDialog { get; set; }
    
    private EmployeeRequestDto _employeeModel { get; set; } = new();
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertEmployeeErrorMessage { get; set; }
    
    private string _deleteEmployeeErrorMessage { get; set; }
    
    private FilterRequestDto Filter = new();
    
    private PagerDto _pagerDto { get; set; } = new();
    
    private async Task HandleFilter()
    {
        Filter.PageSize = 5;
       
        var parameters = new Dictionary<string, string>
        {
            { "companyId", Filter.CompanyId.ToString()! },
            { "search", Filter.Search },
            { "pageNumber", "1" },
            { "pageSize", Filter.PageSize.ToString() },
        };

        var employees = await BaseService.GetAsync<Model.Response.Base.Derived<List<EmployeeResponseDto>>>("employees", parameters);

        _pagerDto = new PagerDto(employees.TotalCount ?? 1, 1, 5);

        _employees = employees.Result ?? [];
        
        Filter.IsInitialized = true;
    }
    
    private async Task OnOrganizationSelection(ChangeEventArgs e)
    {
        if (e.Value == null) return;
        
        var organizationId = Int32.Parse(e.Value.ToString());

        Filter.OrganizationId = organizationId;
        
        

        var parameters = new Dictionary<string, string>
        {
            { "organizationId", organizationId.ToString() },
            { "pageNumber", "1" },
            { "pageSize", "1000" },
        };

        var companies = await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);
            
        _companies = companies?.Result ?? [];
    }
    
    private void OnCompanySelection(ChangeEventArgs e)
    {
        if (e.Value == null) return;
        
        var companyId = Int32.Parse(e.Value.ToString());

        Filter.CompanyId = companyId;
    }
    
    protected override async Task OnInitializedAsync()
    {
        if (_globalState.OrganizationId != null)
        {
            Filter.PageSize = 5;

            var parameters = new Dictionary<string, string>
            {
            { "companyId", _globalState.CompanyId.ToString() },
            { "search", Filter.Search },
            { "pageNumber", "1" },
            { "pageSize", Filter.PageSize.ToString() },
            };

            var employees = await BaseService.GetAsync<Model.Response.Base.Derived<List<EmployeeResponseDto>>>("employees", parameters);

            _pagerDto = new PagerDto(employees.TotalCount ?? 1, 1, 5);

            _employees = employees.Result ?? [];

            Filter.IsInitialized = true;
        }
        else
        {
            var parameters = new Dictionary<string, string>
            {
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var organizations = await BaseService.GetAsync<Model.Response.Base.Derived<List<OrganizationResponseDto>>>("organization", parameters);

            _organizations = organizations!.Result;
        }
    }

    private async Task OpenUpsertEmployeeDialog()
    {
        _dialogTitle = "Add a new employee";

        _dialogOkLabel = "Add";

        _upsertEmployeeErrorMessage = "";

        _showUpsertEmployeeDialog = true;

        _employeeModel = new EmployeeRequestDto();

        if (_globalState.OrganizationId != null)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", _globalState.OrganizationId.ToString()! },
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var organizations = await BaseService.GetAsync<Model.Response.Base.Derived<OrganizationResponseDto>>("organization", parameters);

            _organizations = [organizations!.Result];
        }
        else
        {
            var parameters = new Dictionary<string, string>
            {
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var organizations = await BaseService.GetAsync<Model.Response.Base.Derived<List<OrganizationResponseDto>>>("organization", parameters);

            _organizations = organizations!.Result;
        }
    }

    private async Task HandleOrganizationChange(ChangeEventArgs e)
    {
        var selectedOrganizationId = e.Value?.ToString();

        var result = int.TryParse(selectedOrganizationId, out int organizationId);

        if (result)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", organizationId.ToString() },
            };

            var companies= await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);

            _companies = companies?.Result ?? [];

            _employeeModel.OrganizationId = organizationId;
        }
    }

    private List<CompanyResponseDto>? _companies { get; set; }

    private List<OrganizationResponseDto>? _organizations { get; set; }

    private List<RoleResponseDto> _roles { get; set; }
    private async Task OnUpsertEmployeeDialogClose(bool isClosed)
    {
        try
        {
            if (isClosed)
            {
                _showUpsertEmployeeDialog = false;
            }
            else
            {
                if (string.IsNullOrEmpty(_employeeModel.FullName) || string.IsNullOrEmpty(_employeeModel.EmailAddress) || string.IsNullOrEmpty(_employeeModel.Password) || string.IsNullOrEmpty(_employeeModel.ContactNumber))
                {
                    _upsertEmployeeErrorMessage = "Please fill in all the details.";
            
                    return;
                }
            
                var jsonRequest = JsonSerializer.Serialize(_employeeModel);

                var jsonContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                await BaseService.PostAsync<Model.Response.Base.Derived<object>>("authenticate/register-user", jsonContent);

                _showUpsertEmployeeDialog = false;

                await OnInitializedAsync();
            }
        
            if (Filter.OrganizationId is not 0)
            {
                var parameters = new Dictionary<string, string>
                {
                    { "organizationId", Filter.OrganizationId.ToString()! },
                    { "pageNumber", "1" },
                    { "pageSize", "1000" },
                };

                var companies = await BaseService.GetAsync<Model.Response.Base.Derived<List<CompanyResponseDto>>>("company", parameters);
                
                _companies = companies?.Result ?? [];
            }
        }
        catch (Exception e)
        {
            _upsertEmployeeErrorMessage = e.Message;
        }
        
    }
    
    private async Task HandleCompanyChange(ChangeEventArgs e)
    {
        var selectedCompanyId = e.Value?.ToString();

        var result = int.TryParse(selectedCompanyId, out int companyId);

        if (result)
        {
            var parameters = new Dictionary<string, string>
            {
                { "companyId", companyId.ToString() },
            };
            
            var roles = await BaseService.GetAsync<Model.Response.Base.Derived<List<RoleResponseDto>>>("role", parameters);
            
            _roles = roles?.Result ?? [];

            _employeeModel.CompanyId = companyId;
        }
    }

    private bool _showRoles = false; 
    
    private async Task HandleAdminChange(ChangeEventArgs e)
    {
        var selectedAdminValue = e.Value?.ToString();

        _employeeModel.IsAdmin = selectedAdminValue == "Yes";

        _showRoles = !_employeeModel.IsAdmin.Value;
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
	    
        var parameters = new Dictionary<string, string>
        {
            { "companyId", Filter.CompanyId.ToString() },
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
        };

        var employees = await BaseService.GetAsync<Model.Response.Base.Derived<List<EmployeeResponseDto>>>("employees", parameters);

        _pagerDto = new PagerDto(employees.TotalCount ?? 1, pageNumber, pageSize);

        _employees = employees?.Result ?? [];
    }
}
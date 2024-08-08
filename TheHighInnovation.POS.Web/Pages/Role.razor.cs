using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Model;
using TheHighInnovation.POS.Web.Model.Request.Filter;
using TheHighInnovation.POS.Web.Model.Request.Role;
using TheHighInnovation.POS.Web.Model.Response.Company;
using TheHighInnovation.POS.Web.Model.Response.Module;
using TheHighInnovation.POS.Web.Model.Response.Organization;
using TheHighInnovation.POS.Web.Model.Response.Role;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Role
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    
    private List<RoleResponseDto> _roles { get; set; }

    private int _selectedRoleId { get; set; }
    
    private List<int> _selectedModules { get; set; }
    
    private List<AssignModulePermissionResponseDto> _modules { get; set; }

    private List<OrganizationResponseDto>? _organizations { get; set; }
    
    private List<CompanyResponseDto>? _companies { get; set; }
    
    private bool _showUpsertRoleDialog { get; set; }

    private bool _showUpsertPermissionDialog { get; set; }
    
    private bool _showDeleteRoleDialog { get; set; }
    
    private RoleRequestDto _roleModel { get; set; }
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertRoleErrorMessage { get; set; }
    
    private string _deleteRoleErrorMessage { get; set; }

    private FilterRequestDto Filter = new();
    
    private PagerDto _pagerDto { get; set; } = new();

    private async Task HandleFilter()
    {
        Filter.PageSize = 5;
       
        var parameters = new Dictionary<string, string>
        {
            { "companyId", Filter.CompanyId.ToString()! },
            { "pageNumber", "1" },
            { "pageSize", Filter.PageSize.ToString() },
        };

        var roles = await BaseService.GetAsync<Derived<List<RoleResponseDto>>>("role", parameters);

        _pagerDto = new PagerDto(roles.TotalCount ?? 1, 1, 5);

        _roles = roles.Result ?? [];
        
        Filter.IsInitialized = true;
    }
    
    private async Task OnOrganizationSelection(ChangeEventArgs e)
    {
        if (e.Value == null) return;
        
        var organizationId = Int32.Parse(e.Value.ToString());

        Filter.OrganizationId = organizationId;
        Filter.CompanyId = 0;
        
        var parameters = new Dictionary<string, string>
        {
            { "organizationId", organizationId.ToString() },
            { "pageNumber", "1" },
            { "pageSize", "1000" },
        };

        var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
            
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
        _globalState = await BaseService.GetGlobalState();
        if (_globalState.OrganizationId != null)
        {
            var parameters = new Dictionary<string, string>
            {
                { "organizationId", _globalState.OrganizationId.ToString()! },
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var organizations = await BaseService.GetAsync<Derived<OrganizationResponseDto>>("organization", parameters);

            _organizations = [organizations!.Result];
        }
        else
        {
            var parameters = new Dictionary<string, string>
            {
                { "pageNumber", "1" },
                { "pageSize", "1000" },
            };

            var organizations = await BaseService.GetAsync<Derived<List<OrganizationResponseDto>>>("organization", parameters);

            _organizations = organizations!.Result;
        }
    }
    
    private async Task OpenUpsertRoleDialog(int? roleId = null)
    {
        await OnInitializedAsync();
        
        _dialogTitle = "Add a new role";

        _dialogOkLabel = "Add";

        _upsertRoleErrorMessage = "";

        if (roleId.HasValue)
        {
            var parameters = new Dictionary<string, string>
            {
                { "roleId", roleId.Value.ToString() },
            };

            var result = (await BaseService.GetAsync<Derived<RoleResponseDto>>("role", parameters))?.Result;

            _roleModel = new RoleRequestDto()
            {
                Id = result!.Id,
                Name = result.Name,
                Description = result.Description,
                CompanyId = result.CompanyId,
                Seniority = result.Seniority,
                Type = result.Type
            };
            
            _showUpsertRoleDialog = true;
        }
        else
        {
            _roleModel = new RoleRequestDto();

            if (_globalState.OrganizationId != null)
            {
                var parameters = new Dictionary<string, string>
                {
                    { "organizationId", _globalState.OrganizationId.ToString()! },
                    { "pageNumber", "1" },
                    { "pageSize", "1000" },
                };

                var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
                
                _companies = companies?.Result ?? [];
                
            }
            else
            {
                var parameters = new Dictionary<string, string>
                {
                    { "pageNumber", "1" },
                    { "pageSize", "1000" },
                };

                var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
                
                _companies = companies?.Result ?? [];
            }
            
            _showUpsertRoleDialog = true;
        }
    }

    private async Task OnUpsertRoleDialogClose(bool isClosed)
    {
        try
        {
            if (isClosed)
            {
                _showUpsertRoleDialog = false;
            }
            else
            {
                if (string.IsNullOrEmpty(_roleModel.Name) || _roleModel.CompanyId == 0 ||
                    string.IsNullOrEmpty(_roleModel.Description) || string.IsNullOrEmpty(_roleModel.Seniority))
                {
                    _upsertRoleErrorMessage = "Please fill in all the details";
            
                    return;
                }
            
                var jsonRequest = JsonSerializer.Serialize(_roleModel);

                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                await BaseService.PostAsync<Derived<object>>("role", content);

                _showUpsertRoleDialog = false;
            
            }

            if (Filter.OrganizationId is not 0)
            {
                var parameters = new Dictionary<string, string>
                {
                    { "organizationId", Filter.OrganizationId.ToString()! },
                    { "pageNumber", "1" },
                    { "pageSize", "1000" },
                };

                var companies = await BaseService.GetAsync<Derived<List<CompanyResponseDto>>>("company", parameters);
                
                _companies = companies?.Result ?? [];
            }
        }
        catch (Exception e)
        {
            _upsertRoleErrorMessage = e.Message;
        }
        
        
    }

    private async Task OpenPermissionDialogBox(int roleId)
    {
        _selectedRoleId = roleId;
        
        _showUpsertPermissionDialog = true;
        
        var parameters = new Dictionary<string, string>
        {
            { "roleId", roleId.ToString() },
        };
        
        var result = (await BaseService.GetAsync<Derived<List<AssignModulePermissionResponseDto>>>("permission/get-role-modules", parameters))?.Result;

        _modules = result ?? [];
        
        foreach (var module in _modules)
        {
            module.IsChecked = module.AssignedStatus == "Assigned";
        }
    }
    
    private async Task OnUpsertPermissionDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertPermissionDialog = false;
        }
        else
        {
            var checkedModuleIds = _modules.Where(m => m.IsChecked).Select(m => m.ModuleId).ToList();

            var json = new
            {
                RoleId = _selectedRoleId,
                Modules = checkedModuleIds
            };
            
            var jsonRequest = JsonSerializer.Serialize(json);

            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<string>>("permission/assign-unassign", content);
            
            _showUpsertPermissionDialog = false;
        }
        _organizations = [];
        await OnInitializedAsync();

        _companies = [];
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

        var roles = await BaseService.GetAsync<Derived<List<RoleResponseDto>>>("role", parameters);

        _pagerDto = new PagerDto(roles.TotalCount ?? 1, pageNumber, pageSize);

        _roles = roles?.Result ?? [];
       
    }
}
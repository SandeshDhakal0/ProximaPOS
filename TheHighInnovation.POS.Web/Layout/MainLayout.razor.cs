using Blazored.LocalStorage;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Layout;

public partial class MainLayout
{
    private readonly GlobalState _globalState = new();
    
    protected override async Task OnInitializedAsync()
    {
        var isUserLoggedIn = await BaseService.IsUserLoggedIn();
    
        if (isUserLoggedIn)
        {
            var userDetails = await BaseService.GetAsync<Derived<LoginResponseDetailsDto>>("authenticate/profile");

            if (userDetails?.Result == null || userDetails.Result.UserId == 0)
            {
                await LocalStorageService.RemoveItemAsync("access_token");

                NavigationManager.NavigateTo("/login");
                
                return;
            }

            var result = userDetails.Result;

            _globalState.UserId = result.UserId;
            _globalState.EmployeeId = result.EmployeeId;
            _globalState.Name = result.Name;
            _globalState.RoleId = result.RoleId;
            _globalState.RoleName = result.RoleName;
            _globalState.RoleType = result.RoleType;
            _globalState.CompanyId = result.CompanyId;
            _globalState.CompanyName = result.CompanyName;
            _globalState.OrganizationId = result.OrganizationId;
            _globalState.OrganizationName = result.OrganizationName;
            _globalState.IsAdmin = result.IsAdmin;
            _globalState.IsSuperAdmin = result.IsSuperAdmin;
        }
        else
        {
            await LocalStorageService.RemoveItemAsync("access_token");

            NavigationManager.NavigateTo("/login");
        }
    }
    
}
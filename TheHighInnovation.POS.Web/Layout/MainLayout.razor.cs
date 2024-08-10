using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Routing;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Layout;

public partial class MainLayout
{
    private readonly GlobalState _globalState = new();
    private bool ShowSidebar { get; set; }

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

            DetermineSidebarVisibility(NavigationManager.Uri);
            NavigationManager.NavigateTo("/landing-page");
        }
        else
        {
            await LocalStorageService.RemoveItemAsync("access_token");
            NavigationManager.NavigateTo("/login");
        }

        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
    {
        DetermineSidebarVisibility(e.Location);
        StateHasChanged();
    }

    private void DetermineSidebarVisibility(string location)
    {
        // Define routes where you want the sidebar
        ShowSidebar = location.Contains("/category") ||
                      location.Contains("/vendor") ||
                      location.Contains("/module") ||
                      location.Contains("/service") ||
                      location.Contains("/organization") ||
                      location.Contains("/company") ||
                      location.Contains("/role") ||
                      location.Contains("/inventory-category") ||
                      location.Contains("/inventory-unit") ||
                      location.Contains("/product-service") ||
                      location.Contains("/productmanagement") ||
                      location.Contains("/kharidkhata") ||
                      location.Contains("/bikrikhata") ||
                      location.Contains("/inventoryrecords") ||
                      location.Contains("/safe-drop-report") ||
                      location.Contains("/void-report") ||
                      location.Contains("/lock-report") ||
                      location.Contains("/petty-cash-report") ||
                      location.Contains("/product") ||
                      location.Contains("/employees"); 
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }

}
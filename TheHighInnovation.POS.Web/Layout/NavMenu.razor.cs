using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Layout;

public partial class NavMenu
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }
    
    private string currentDate = DateTime.Now.ToString("dd-MM-yyyy");

    protected override async Task OnInitializedAsync()
    {
	    _globalState = await BaseService.GetGlobalState();
    }
    
    private async Task LogoutHandler()
    {
        _globalState = new GlobalState();

        await BaseService.LogOutUser();      
        
        NavManager.NavigateTo("/login", forceLoad: true);
    }
    
	private bool isDropdownOpen = false;

	private void ToggleDropdown()
	{
		isDropdownOpen = !isDropdownOpen;
		
        StateHasChanged();
	}
	
	private void SwitchToSettingsView()
    {
        _globalState.PageNumber = 0;
        
        NavManager.NavigateTo("/landing-page");
        
        StateHasChanged();
    }
}
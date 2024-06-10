using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Models;


namespace TheHighInnovation.POS.Web.Layout;

public partial class NavPanel
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }

    private bool _isDropdownOpen = false;

	private void ToggleDropdown()
	{
		_isDropdownOpen = !_isDropdownOpen;
	}

	private async Task LogoutHandler()
    {
        _globalState = new GlobalState();

        await BaseService.LogOutUser();      
        
        NavManager.NavigateTo("/login", forceLoad: true);
    }
}
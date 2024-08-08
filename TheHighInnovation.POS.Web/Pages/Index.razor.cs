using TheHighInnovation.POS.Web.Model;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Index
{
    [CascadingParameter] private GlobalState? GlobalState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        GlobalState = await BaseService.GetGlobalState();

        var navigation = GlobalState?.UserId == 0 ? "/login" : GlobalState?.RoleType == 0
            ? GlobalState?.OrganizationId == null || GlobalState.OrganizationId == 0
                ? "/admin-dashboard"
                : "/landing-page"
            : "/cashier-corner";
        
        NavManager.NavigateTo(navigation);
    }
}
using TheHighInnovation.POS.Model;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.Web.Models;

namespace TheHighInnovation.POS.Web.Pages;

public partial class Index
{
    [CascadingParameter] private GlobalState? GlobalState { get; set; }

    protected override void OnInitialized()
    {
        var navigation = GlobalState?.UserId == 0 ? "/login" : GlobalState?.RoleType == 0 ? "/admin-dashboard" : "/cashier-corner";

        NavManager.NavigateTo(navigation);
    }
}
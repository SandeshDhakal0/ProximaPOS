using Microsoft.AspNetCore.Components;

namespace TheHighInnovation.POS.Web.Layout;

public partial class AlertMessage
{
    [Parameter] public string Type { get; set; }
    
    [Parameter] public string Message { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(2000);        
        
        Message = string.Empty;
        
        Type = string.Empty;
        
        StateHasChanged();
    }
}
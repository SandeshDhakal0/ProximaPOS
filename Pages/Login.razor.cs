using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TheHighInnovation.POS.WEB.Model;
using TheHighInnovation.POS.WEB.Model.Request.Authentication;
using TheHighInnovation.POS.WEB.Model.Response.Authentication;
using TheHighInnovation.POS.WEB.Model.Response.Base;
using TheHighInnovation.POS.WEB.Services;




namespace TheHighInnovation.POS.WEB.Pages;

public partial class Login
{
    [CascadingParameter]
    private GlobalState? GlobalState { get; set; }
    
    private string _message = "";
    private readonly LoginRequestDto _loginRequestDto = new();
    private readonly StorageService _storageService;


    private async Task HandleValidSubmit()
    {
        try
        {

            var loginRequest = new LoginRequestDto
            {
                EmailAddress = _loginRequestDto.EmailAddress,
                Password = _loginRequestDto.Password
            };

            var jsonRequest = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            var authResult = await _baseService.PostAsync<Derived<LoginResponseDto>>("authenticate/login", content);

            await _storageService.SetItemAsync("access_token", authResult?.Result.Token ?? "");

            if (GlobalState != null)
            {
                GlobalState.UserId = authResult?.Result.UserId ?? 0;
                GlobalState.Name = authResult?.Result.Name ?? string.Empty;
                GlobalState.RoleId = authResult?.Result.RoleId ?? 0;
                GlobalState.RoleName = authResult?.Result.RoleName ?? string.Empty;
                GlobalState.RoleType = authResult?.Result.RoleType ?? 0;
                GlobalState.EmployeeId = authResult?.Result.EmployeeId;
                GlobalState.CompanyId = authResult?.Result.CompanyId;
                GlobalState.CompanyName = authResult?.Result.CompanyName;
                GlobalState.OrganizationId = authResult?.Result.OrganizationId;
                GlobalState.OrganizationName = authResult?.Result.OrganizationName;
                GlobalState.IsAdmin = authResult?.Result.IsAdmin ?? false;
                GlobalState.IsSuperAdmin = authResult?.Result.IsSuperAdmin ?? false;
            }

            var navigation = GlobalState?.RoleType == 0
                ? GlobalState?.OrganizationId == null || GlobalState.OrganizationId == 0
                    ? "/admin-dashboard"
                    : "/landing-page"
                : "/cashier-corner";

            GlobalState.PageNumber = navigation == "/admin-dashboard" ? 1 : 0;
            NavigationManager.NavigateTo(navigation);
        }
        catch (Exception ex)
        {
            _message = "The username or password is incorrect. Try again.";
            Console.WriteLine(ex);
        }        
    }



    /* private string QRCodeResult = "Click Capture To Read QR or Bar Code";

     protected override async Task OnInitializedAsync()
     {
         await _jsRuntime.InvokeVoidAsync("initializeCamera");
     }
     private async Task SwitchCAM()
     {
         await _jsRuntime.InvokeVoidAsync("SwitchCam");
     }

     private async Task PlayCAM()
     {
         await _jsRuntime.InvokeVoidAsync("PlayCam");
     }

     private async Task PauseCAM()
     {
         await _jsRuntime.InvokeVoidAsync("PauseCam");
     }

     private async Task CaptureFrame()
     {
         await _jsRuntime.InvokeAsync<String>("getFrame", DotNetObjectReference.Create(this));
     }

     [JSInvokable]
     public async void ProcessImage(string imageString)
     {
         var imageObject = new CameraImage
         {
             ImageDataBase64 = imageString
         };

         var jsonObj = JsonSerializer.Serialize(imageObject);

         var barcodeResult = await Http.PostAsJsonAsync($"WeatherForecast/ReadBarCode", jsonObj);

         if (barcodeResult.StatusCode != System.Net.HttpStatusCode.OK) return;

         QRCodeResult = barcodeResult.Content.ReadAsStringAsync().Result;

         StateHasChanged();
     }*/

}
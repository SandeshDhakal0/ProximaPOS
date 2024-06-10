using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheHighInnovation.POS.Web;
using TheHighInnovation.POS.Web.Services;
using TheHighInnovation.POS.Web.Services.Base;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = AppConfiguration.BaseUrl;

builder.Services.AddScoped(x => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

builder.Services.AddTransient<BaseService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
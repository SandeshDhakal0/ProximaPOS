using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheHighInnovation.POS.WEB;
using TheHighInnovation.POS.WEB.Services;
using TheHighInnovation.POS.WEB.Services.Base;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = AppConfiguration.BaseUrl;

services.AddScoped(x => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

services.AddTransient<BaseService>();
builder.Services.AddScoped<StorageService>();




await builder.Build().RunAsync();

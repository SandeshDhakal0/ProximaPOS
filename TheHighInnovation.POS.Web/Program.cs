using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheHighInnovation.POS.Web;
using TheHighInnovation.POS.Web.Models;
using TheHighInnovation.POS.Web.Services;
using TheHighInnovation.POS.Web.Services.Alert;
using TheHighInnovation.POS.Web.Services.Base;
using TheHighInnovation.POS.Web.Services.Loader;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

var configuration = builder.Configuration;

var applicationConfiguration = configuration.GetSection("Configuration").Get<Configuration>()!;

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(applicationConfiguration.BaseUrl)
});

builder.Services.AddTransient<BaseService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddBlazorBootstrap();

builder.Services.AddTransient<BaseService>();

builder.Services.AddScoped<SweetAlert>();

builder.Services.AddScoped<LoaderService>();

await builder.Build().RunAsync();
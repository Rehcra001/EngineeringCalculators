using EngineeringCalculators.Web;
using EngineeringCalculators.Web.Services;
using EngineeringCalculators.Web.Services.Contracts;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddFileSystemAccessService();
builder.Services.AddScoped<IMaterialService, MaterialService>();

await builder.Build().RunAsync();

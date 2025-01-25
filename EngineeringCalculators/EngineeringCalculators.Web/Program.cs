using EngineeringCalculators.Web;
using EngineeringCalculators.Web.Services;
using EngineeringCalculators.Web.Services.Contracts;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EngineeringCalculators.Web.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//EngCalcDb database
builder.Services.AddSingleton<EngineeringCalculatorsDb>();

builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddFileSystemAccessService();
builder.Services.AddTransient<IIndexedDbService, IndexedDbService>();
builder.Services.AddTransient<IBackupIndexedDbService, BackupIndexedDbService>();
builder.Services.AddTransient<IRestoreIndexedDbService, RestoreIndexedDbService>();

await builder.Build().RunAsync();

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

//IndexedDb database
builder.Services.AddSingleton<EngineeringCalculatorsDb>();
builder.Services.AddSingleton<FolderAndFileHandlesDb>();

builder.Services.AddFileSystemAccessService();

// TODO: Look at removing and only use for backing up the indexedDb database
builder.Services.AddTransient<IMaterialFileAccessService, MaterialFileAccessService>();
builder.Services.AddTransient<IMaterialndexedDbService, MaterialndexedDbService>();
builder.Services.AddTransient<IBackupRestoreIndexedDbService, BackupRestoreIndexedDbService>();

await builder.Build().RunAsync();

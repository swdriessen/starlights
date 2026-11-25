using Starlights.Modules.FileManagement;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddFileManagementModule();
using var host = builder.Build();
await host.Services.InitializeFileManagementAsync();

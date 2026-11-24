using Starlights.Modules.Identity;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddIdentityModule();
using var host = builder.Build();
await host.Services.MigrateIdentityAsync();

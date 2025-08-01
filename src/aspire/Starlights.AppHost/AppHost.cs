var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Starlights_Application>("starlights-application");

builder.Build().Run();

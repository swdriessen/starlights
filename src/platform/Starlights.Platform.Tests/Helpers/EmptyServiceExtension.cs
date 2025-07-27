using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Tests.Helpers;

internal class EmptyServiceExtension : IPlatformServicesExtension
{
    public virtual int RegistrationOrder => 1001;

    public virtual void ConfigureServices(IHostApplicationBuilder builder)
    {
        // build the service provider to invoke the mock in order to verify the extension is invoked
        var provider = builder.Services.BuildServiceProvider();
        var service = provider.GetService<IEmptyVerificationService>();
        service?.TestConfigureServices(this);
    }
}

internal sealed class BeforeEmptyServiceExtension : EmptyServiceExtension
{
    public override int RegistrationOrder => base.RegistrationOrder - 5;
}

internal sealed class AfterEmptyServiceExtension : EmptyServiceExtension
{
    public override int RegistrationOrder => base.RegistrationOrder + 5;
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Tests.Helpers;

internal class EmptyApplicationExtension : IPlatformApplicationComponent
{
    public virtual int RegistrationOrder => 1000;

    public void UseComponent(IHost host)
    {
        // build the service provider to invoke the mock in order to verify the extension is invoked        
        var service = host.Services.GetService<IEmptyVerificationService>();
        service?.TestConfigureApplication(this);
    }
}

internal sealed class BeforeEmptyApplicationExtension : EmptyApplicationExtension
{
    public override int RegistrationOrder => base.RegistrationOrder - 5;
}

internal sealed class AfterEmptyApplicationExtension : EmptyApplicationExtension
{
    public override int RegistrationOrder => base.RegistrationOrder + 5;
}
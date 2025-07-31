using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Starlights.Application;

namespace Starlights.Integration.Tests.Elements;

[TestClass]
public sealed class ElementsInstallationTests
{
    [TestMethod]
    public async Task Initialize()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Integration"));
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/elements/initialize", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}

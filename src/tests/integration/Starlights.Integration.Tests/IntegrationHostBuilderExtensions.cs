using System.Reflection;

namespace Starlights.Integration;

public static class IntegrationHostBuilderExtensions2
{
    extension(IntegrationHost host)
    {
        /// <summary>
        /// Creates a default <see cref="IntegrationHostBuilder"/> for the specified <see cref="IntegrationTestBase"/>.
        /// </summary>
        /// <remarks>
        /// This method configures the builder with the current assembly's driver assemblies and the test context from the provided test base.
        /// </remarks>
        public static IntegrationHostBuilder CreateDefaultBuilder(IntegrationTestBase testBase)
        {
            return new IntegrationHostBuilder()
                .WithDriverAssemblies(Assembly.GetExecutingAssembly())
                .WithTestContext(testBase.TestContext)
                ;
        }
    }
}

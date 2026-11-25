using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Data.EntityFramework;
using Starlights.Platform.Hosting;
using MigrationProgram = Modules.Characters.Data.EntityFramework.MigrationService.Program;

[assembly: DoNotParallelize]

namespace Starlights.Modules.Characters.Data.EntityFramework.MigrationService.Tests;

[TestClass]
public sealed class MigrationHostTests
{
    [TestMethod]
    public async Task CreateHost_OutsideAnAuthenticatedRequest_ResolvesCharacterPersistenceWithoutStartingMigrations()
    {
        using var host = MigrationProgram.CreateHost([
            "--environment", "Development",
            "--ConnectionStrings:starlights-db", "Server=127.0.0.1,1;Database=MigrationHostTests;Integrated Security=true"
        ]);
        using var scope = host.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ICharactersRepository>();
        using var context = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CharactersContext>>().CreateDbContext();
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.SqlServer");
        scope.ServiceProvider.GetRequiredService<ICurrentUser>().UserId.Should().BeNull();

        Func<Task> listCharacters = async () => await repository.GetCharactersAsync();
        await listCharacters.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Listing Characters requires an authenticated owner.");
    }
}

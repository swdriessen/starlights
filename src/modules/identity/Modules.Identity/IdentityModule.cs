using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.Identity.Accounts;
using Starlights.Modules.Identity.Data;
using Starlights.Modules.Identity.Features;
using Starlights.Modules.Identity.Security;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Identity;

/// <summary>Registers isolated account storage and standard authentication/authorization integration.</summary>
public static class IdentityModule
{
    /// <summary>Registers Identity with durable SQL Server storage and browser cookie sessions.</summary>
    public static void AddIdentityModule(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("starlights-db");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Identity requires the 'starlights-db' SQL Server connection string.");
            }
            options.UseSqlServer(connectionString, sql => sql
                .MigrationsHistoryTable("__EFMigrationsHistory", "identity")
                .EnableRetryOnFailure());
        });

        builder.Services.AddDataProtection().SetApplicationName("Starlights")
            .PersistKeysToDbContext<IdentityContext>();
        builder.Services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddSignInManager<IdentitySignInManager>()
            .AddDefaultTokenProviders();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        }).AddIdentityCookies();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "Starlights.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
        builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);
        builder.Services.TryAddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<ITicketStore, DatabaseTicketStore>();
        builder.Services.AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
            .Configure<TimeProvider, ITicketStore>((options, time, tickets) =>
            {
                options.TimeProvider = time;
                options.SessionStore = tickets;
            });
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "Starlights.Antiforgery";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.TryAddSingleton<ICurrentUser, HttpContextCurrentUser>();
        builder.Services.AddScoped<IIdentitySessions, IdentitySessions>();
        builder.Services.AddScoped<IAccountAdministration, AccountAdministration>();
    }

    /// <summary>Protects API requests, including retained legacy routes, with authentication and antiforgery.</summary>
    public static void UseIdentityApiProtection(this WebApplication app)
        => app.UseMiddleware<ApiAccessMiddleware>();

    /// <summary>Maps the Minimal API sign-in, session, sign-out and antiforgery features.</summary>
    public static void MapIdentity(this WebApplication app) => app.MapIdentityEndpoints();

    /// <summary>Applies only the Identity module's migrations; startup failures propagate to the caller.</summary>
    public static async Task MigrateIdentityAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.MigrateAsync(cancellationToken);
    }
}

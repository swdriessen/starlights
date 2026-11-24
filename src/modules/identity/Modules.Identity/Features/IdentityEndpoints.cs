using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Starlights.Modules.Identity.Features.Antiforgery;
using Starlights.Modules.Identity.Features.Session;
using Starlights.Modules.Identity.Features.SignIn;

namespace Starlights.Modules.Identity.Features;

internal static class IdentityEndpoints
{
    internal static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity").WithTags("Identity");

        group.MapGet("/antiforgery", (HttpContext context, IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            return TypedResults.Ok(new AntiforgeryResponse(
                tokens.RequestToken ?? throw new InvalidOperationException("No antiforgery token was generated."),
                tokens.HeaderName ?? throw new InvalidOperationException("No antiforgery header was configured.")));
        }).AllowAnonymous().WithName("GetAntiforgeryToken")
            .WithSummary("Get a token for the current browser session before an API write.");

        group.MapPost("/sign-in", async Task<Results<NoContent, ProblemHttpResult>> (
            SignInRequest request, IIdentitySessions sessions, CancellationToken cancellationToken) =>
        {
            return await sessions.SignInAsync(request.Username, request.Password, cancellationToken)
                ? TypedResults.NoContent()
                : TypedResults.Problem(statusCode: StatusCodes.Status401Unauthorized,
                    title: "Sign-in failed. Check your credentials or contact the operator.");
        }).AllowAnonymous().WithName("SignIn")
            .WithSummary("Sign in to an operator-provisioned local account.")
            .ProducesValidationProblem().ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/session", (IIdentitySessions sessions) => TypedResults.Ok(sessions.GetSession()))
            .RequireAuthorization().WithName("GetSession")
            .WithSummary("Get the current account's stable identity.")
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/sign-out", async (IIdentitySessions sessions, CancellationToken cancellationToken) =>
        {
            await sessions.SignOutAsync(cancellationToken);
            return TypedResults.NoContent();
        }).RequireAuthorization().WithName("SignOut")
            .WithSummary("End the current browser session.")
            .ProducesProblem(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status401Unauthorized);
    }
}

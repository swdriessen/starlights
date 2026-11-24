using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Starlights.Modules.Identity.Security;

internal sealed class ApiAccessMiddleware(RequestDelegate next, ILogger<ApiAccessMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        context.Response.Headers.CacheControl = "no-store";
        var anonymous = context.Request.Path.Equals("/api/identity/antiforgery")
            || context.Request.Path.Equals("/api/identity/sign-in");

        if (!anonymous && context.User.Identity?.IsAuthenticated != true)
        {
            await context.ChallengeAsync();
            return;
        }

        if (!HttpMethods.IsGet(context.Request.Method)
            && !HttpMethods.IsHead(context.Request.Method)
            && !HttpMethods.IsOptions(context.Request.Method))
        {
            try
            {
                await antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException)
            {
                logger.LogWarning("Rejected an API write with an invalid antiforgery token");
                await Results.Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: "A valid antiforgery token is required.").ExecuteAsync(context);
                return;
            }
        }

        await next(context);
    }
}

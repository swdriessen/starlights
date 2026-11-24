namespace Starlights.Modules.Identity.Features.Antiforgery;

/// <summary>Supplies a request token to pair with the browser's antiforgery cookie.</summary>
/// <param name="RequestToken">The token to send with a state-changing request.</param>
/// <param name="HeaderName">The request header carrying the token.</param>
public sealed record AntiforgeryResponse(string RequestToken, string HeaderName);

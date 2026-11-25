using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Starlights.Modules.FileManagement.Middleware;

internal sealed class FileUploadLimitMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method)
            && (context.Request.Path.Equals("/api/files") || context.Request.Path.Equals("/api/files/"))
            && context.Features.Get<IHttpMaxRequestBodySizeFeature>() is { IsReadOnly: false } limit)
            limit.MaxRequestBodySize = 8 * 1024 * 1024;
        return next(context);
    }
}

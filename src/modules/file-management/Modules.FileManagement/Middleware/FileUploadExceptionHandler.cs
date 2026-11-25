using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Starlights.Modules.FileManagement.Features;

namespace Starlights.Modules.FileManagement.Middleware;

internal sealed class FileUploadExceptionHandler(ILogger<FileUploadExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not FileUploadException upload)
            return false;
        logger.LogWarning("Rejected private file upload: {Reason}", upload.Message);
        await TypedResults.Problem(statusCode: upload.StatusCode, title: "Invalid private file upload.", detail: upload.Message).ExecuteAsync(context);
        return true;
    }
}

namespace Starlights.Modules.FileManagement.Features;

internal sealed class FileUploadException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

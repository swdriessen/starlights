using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Starlights.Modules.FileManagement.Features;

internal static class FileEndpoints
{
    public static void MapPrivateFiles(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/files").WithTags("Private files").RequireAuthorization();
        group.MapPost("", async (UploadFileRequest request, IPrivateFiles files, CancellationToken cancellationToken) =>
        {
            var file = await files.UploadAsync(request, cancellationToken);
            return TypedResults.Created($"/api/files/{file.Id}", file);
        }).WithName("UploadPrivateFile").WithSummary("Upload a private immutable image.")
            .WithDescription("Accepts original PNG, JPEG or WebP bytes as base64, up to 5 MiB and 4096 pixels per side. Animated images are rejected.")
            .ProducesValidationProblem().ProducesProblem(400).ProducesProblem(413).Produces(401);

        group.MapGet("", async Task<Results<Ok<FileListResponse>, ProblemHttpResult>> (
            IPrivateFiles files, CancellationToken cancellationToken, int offset = 0) =>
        {
            if (offset < 0 || offset > int.MaxValue - 50)
                return TypedResults.Problem(statusCode: 400, title: "The file offset is outside the supported range.");
            return TypedResults.Ok(await files.ListAsync(offset, cancellationToken));
        }).WithName("ListPrivateFiles").WithSummary("List a page of the authenticated owner's files.").Produces(401);

        group.MapGet("/{fileId:guid}", async Task<Results<Ok<FileResponse>, NotFound>> (
            Guid fileId, IPrivateFiles files, CancellationToken cancellationToken) =>
            await files.GetAsync(fileId, cancellationToken) is { } file ? TypedResults.Ok(file) : TypedResults.NotFound())
            .WithName("GetPrivateFile").WithSummary("Inspect an owned file; other owners receive 404.").Produces(401);

        group.MapGet("/{fileId:guid}/content", async Task<Results<FileStreamHttpResult, NotFound>> (
            Guid fileId, IPrivateFiles files, HttpContext context, CancellationToken cancellationToken) =>
        {
            var file = await files.OpenReadAsync(fileId, cancellationToken);
            if (file is null)
                return TypedResults.NotFound();
            context.Response.Headers.XContentTypeOptions = "nosniff";
            return TypedResults.File(file.Stream, file.ContentType);
        }).WithName("ReadPrivateFile").WithSummary("Read original file bytes through owner-authorized access.")
            .Produces(200, typeof(byte[]), "image/png", "image/jpeg", "image/webp").Produces(401);

        group.MapDelete("/{fileId:guid}", async Task<Results<NoContent, NotFound>> (
            Guid fileId, IPrivateFiles files, CancellationToken cancellationToken) =>
            await files.DeleteAsync(fileId, cancellationToken) ? TypedResults.NoContent() : TypedResults.NotFound())
            .WithName("DeletePrivateFile").WithSummary("Delete an owned file and its stored content.")
            .ProducesProblem(400).Produces(401);
    }
}

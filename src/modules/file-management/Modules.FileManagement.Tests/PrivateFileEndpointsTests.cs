using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using Starlights.Modules.FileManagement.Data;
using Starlights.Modules.FileManagement.Features;

[assembly: DoNotParallelize]

namespace Starlights.Modules.FileManagement.Tests;

[TestClass]
public sealed class PrivateFileEndpointsTests(TestContext testContext)
{
    private string _directory = null!;
    private FileApplicationFactory _factory = null!;
    private readonly Guid _owner = Guid.NewGuid();

    [TestInitialize]
    public async Task Initialize()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"Starlights.Files.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
        _factory = new FileApplicationFactory(Path.Combine(_directory, "files.db"));
        await _factory.InitializeAsync(testContext.CancellationToken);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _factory.DisposeAsync();
        Directory.Delete(_directory, recursive: true);
    }

    [TestMethod]
    [DataRow(SKEncodedImageFormat.Png, "image/png")]
    [DataRow(SKEncodedImageFormat.Jpeg, "image/jpeg")]
    [DataRow(SKEncodedImageFormat.Webp, "image/webp")]
    public async Task Upload_ReadAndDelete_PreserveOriginalBytesAndDetectedType(SKEncodedImageFormat format, string mediaType)
    {
        var bytes = Image(format);
        using var browser = _factory.Browser(_owner);
        var file = await Upload(browser, bytes, "untrusted-extension.txt");

        file.ContentType.Should().Be(mediaType);
        file.SizeBytes.Should().Be(bytes.Length);
        file.FileName.Should().Be("untrusted-extension.txt");
        using var read = await browser.GetAsync(file.ContentUrl, testContext.CancellationToken);
        read.EnsureSuccessStatusCode();
        read.Content.Headers.ContentType!.MediaType.Should().Be(mediaType);
        read.Headers.GetValues("X-Content-Type-Options").Should().Contain("nosniff");
        read.Headers.CacheControl!.NoStore.Should().BeTrue();
        (await read.Content.ReadAsByteArrayAsync(testContext.CancellationToken)).Should().Equal(bytes);

        var second = await Upload(browser, bytes, "replacement.png");
        second.Id.Should().NotBe(file.Id);
        using var deleted = await Write(browser, HttpMethod.Delete, $"/api/files/{file.Id}");
        deleted.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var absent = await browser.GetAsync(file.ContentUrl, testContext.CancellationToken);
        absent.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var replacement = await browser.GetAsync(second.ContentUrl, testContext.CancellationToken);
        replacement.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task OtherOwners_CannotListInspectReadOrDelete()
    {
        using var owner = _factory.Browser(_owner);
        var file = await Upload(owner, Image());
        using var other = _factory.Browser(Guid.NewGuid());

        var list = await other.GetFromJsonAsync<FileListResponse>("/api/files", testContext.CancellationToken);
        list!.Files.Should().BeEmpty();
        using var metadata = await other.GetAsync($"/api/files/{file.Id}", testContext.CancellationToken);
        using var bytes = await other.GetAsync(file.ContentUrl, testContext.CancellationToken);
        using var deleted = await Write(other, HttpMethod.Delete, $"/api/files/{file.Id}");
        metadata.StatusCode.Should().Be(HttpStatusCode.NotFound);
        bytes.StatusCode.Should().Be(HttpStatusCode.NotFound);
        deleted.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var preserved = await owner.GetAsync(file.ContentUrl, testContext.CancellationToken);
        preserved.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task Requests_RequireAuthenticationAndAntiforgery()
    {
        using var anonymous = _factory.Browser();
        using var denied = await anonymous.GetAsync("/api/files", testContext.CancellationToken);
        denied.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        denied.Headers.Location.Should().BeNull();
        using var owner = _factory.Browser(_owner);
        using var noToken = await owner.PostAsJsonAsync("/api/files", Request(Image()), testContext.CancellationToken);
        noToken.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var badOffset = await owner.GetAsync("/api/files?offset=-1", testContext.CancellationToken);
        badOffset.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _factory.Content.Count.Should().Be(0);
    }

    [TestMethod]
    [DataRow("svg")]
    [DataRow("truncated")]
    [DataRow("truncated-png-tail")]
    [DataRow("truncated-jpeg-tail")]
    [DataRow("truncated-webp-tail")]
    [DataRow("oversized")]
    [DataRow("dimensions")]
    [DataRow("dimensions-height")]
    [DataRow("invalid-base64")]
    public async Task InvalidUploads_AreRejectedWithoutStoredContent(string scenario)
    {
        var bytes = scenario switch
        {
            "svg" => Encoding.UTF8.GetBytes("<svg xmlns=\"http://www.w3.org/2000/svg\"/>"),
            "truncated" => Image()[..25],
            "truncated-png-tail" => Image()[..^8],
            "truncated-jpeg-tail" => Image(SKEncodedImageFormat.Jpeg)[..^2],
            "truncated-webp-tail" => Image(SKEncodedImageFormat.Webp)[..^3],
            "oversized" => new byte[ImageUploadValidator.MaximumBytes + 1],
            "dimensions" => Image(width: ImageUploadValidator.MaximumDimension + 1),
            "dimensions-height" => Image(height: ImageUploadValidator.MaximumDimension + 1),
            _ => Image()
        };
        var request = Request(bytes);
        if (scenario == "invalid-base64")
            request = request with { ContentBase64 = "not-valid-base64" };
        using var browser = _factory.Browser(_owner);
        using var response = await Write(browser, HttpMethod.Post, "/api/files", request);
        response.StatusCode.Should().Be(scenario == "oversized" ? HttpStatusCode.RequestEntityTooLarge : HttpStatusCode.BadRequest);
        _factory.Content.Count.Should().Be(0);
        var list = await browser.GetFromJsonAsync<FileListResponse>("/api/files", testContext.CancellationToken);
        list!.Files.Should().BeEmpty();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task DeletionFailure_RemainsVisibleAndRetryable(bool metadataFailure)
    {
        using var browser = _factory.Browser(_owner);
        var file = await Upload(browser, Image());
        _factory.Content.FailNextDeletion = !metadataFailure;
        _factory.DeletionFailure.FailNextDelete = metadataFailure;
        using var failed = await Write(browser, HttpMethod.Delete, $"/api/files/{file.Id}");
        failed.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var pending = await browser.GetFromJsonAsync<FileResponse>($"/api/files/{file.Id}", testContext.CancellationToken);
        pending!.IsDeleting.Should().BeTrue();
        _factory.Content.Count.Should().Be(metadataFailure ? 0 : 1);
        using var hiddenContent = await browser.GetAsync(file.ContentUrl, testContext.CancellationToken);
        hiddenContent.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var retry = await Write(browser, HttpMethod.Delete, $"/api/files/{file.Id}");
        retry.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _factory.Content.Count.Should().Be(0);
    }

    [TestMethod]
    [DataRow("png")]
    [DataRow("webp")]
    [DataRow("webp-single-frame")]
    public async Task AnimatedImages_AreRejected(string format)
    {
        var bytes = format == "png" ? AnimatedImageFixtures.Png() : AnimatedImageFixtures.Webp(includeSecondFrame: format != "webp-single-frame");
        using var browser = _factory.Browser(_owner);
        using var response = await Write(browser, HttpMethod.Post, "/api/files", Request(bytes));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(testContext.CancellationToken);
        problem.GetProperty("detail").GetString().Should().Be("Animated images are not supported.");
        _factory.Content.Count.Should().Be(0);
    }

    [TestMethod]
    public async Task MaximumDimension_IsAccepted()
    {
        using var browser = _factory.Browser(_owner);
        var bytes = Image(width: ImageUploadValidator.MaximumDimension, height: ImageUploadValidator.MaximumDimension);
        var file = await Upload(browser, bytes);
        file.SizeBytes.Should().Be(bytes.Length);
    }

    [TestMethod]
    public async Task MaximumFileSize_IsAcceptedWithOriginalMetadata()
    {
        var image = Image();
        using var output = new MemoryStream();
        output.Write(image.AsSpan(0, image.Length - 12));
        var metadata = new byte[ImageUploadValidator.MaximumBytes - image.Length - 12];
        Array.Fill(metadata, (byte)'a');
        metadata[0] = (byte)'x';
        metadata[1] = 0;
        AnimatedImageFixtures.WritePngChunk(output, "tEXt", metadata);
        output.Write(image.AsSpan(image.Length - 12));
        using var browser = _factory.Browser(_owner);
        var file = await Upload(browser, output.ToArray());
        file.SizeBytes.Should().Be(ImageUploadValidator.MaximumBytes);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task MetadataFailure_CompensatesContentOrSurfacesCleanupFailure(bool cleanupFails)
    {
        using var browser = _factory.Browser(_owner);
        _factory.PersistenceFailure.FailNextSave = true;
        _factory.Content.FailNextDeletion = cleanupFails;
        using var response = await Write(browser, HttpMethod.Post, "/api/files", Request(Image()));
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        _factory.Content.Count.Should().Be(cleanupFails ? 1 : 0);
        var list = await browser.GetFromJsonAsync<FileListResponse>("/api/files", testContext.CancellationToken);
        list!.Files.Should().BeEmpty();
    }

    [TestMethod]
    public async Task List_ProvidesBoundedPagesWithoutLeakingAnotherOwner()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<FileManagementContext>();
            for (var index = 0; index < 52; index++)
                context.Files.Add(new StoredFile(Guid.NewGuid(), index == 51 ? Guid.NewGuid() : _owner,
                    $"file-{index}.png", "image/png", 100, DateTimeOffset.UtcNow.AddSeconds(-index)));
            await context.SaveChangesAsync(testContext.CancellationToken);
        }
        using var browser = _factory.Browser(_owner);
        var first = await browser.GetFromJsonAsync<FileListResponse>("/api/files", testContext.CancellationToken);
        first!.Files.Should().HaveCount(50);
        first.NextOffset.Should().Be(50);
        var second = await browser.GetFromJsonAsync<FileListResponse>($"/api/files?offset={first.NextOffset}", testContext.CancellationToken);
        second!.Files.Should().ContainSingle();
        second.NextOffset.Should().BeNull();
        first.Files.Select(file => file.Id).Should().NotContain(second.Files.Single().Id);
    }

    private async Task<FileResponse> Upload(HttpClient browser, byte[] bytes, string name = "portrait.png")
    {
        using var response = await Write(browser, HttpMethod.Post, "/api/files", Request(bytes) with { FileName = name });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var file = await response.Content.ReadFromJsonAsync<FileResponse>(testContext.CancellationToken);
        file.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be($"/api/files/{file!.Id}");
        return file;
    }

    private async Task<HttpResponseMessage> Write(HttpClient browser, HttpMethod method, string path, UploadFileRequest? body = null)
    {
        var token = await browser.GetFromJsonAsync<JsonElement>("/api/identity/antiforgery", testContext.CancellationToken);
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Add(token.GetProperty("headerName").GetString()!, token.GetProperty("requestToken").GetString());
        if (body is not null)
            request.Content = JsonContent.Create(body);
        return await browser.SendAsync(request, testContext.CancellationToken);
    }

    private static UploadFileRequest Request(byte[] bytes) => new() { FileName = "portrait.png", ContentBase64 = Convert.ToBase64String(bytes) };

    private static byte[] Image(SKEncodedImageFormat format = SKEncodedImageFormat.Png, int width = 2, int height = 2)
    {
        using var bitmap = new SKBitmap(width, height);
        bitmap.Erase(SKColors.CornflowerBlue);
        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(format, 90);
        return encoded.ToArray();
    }
}

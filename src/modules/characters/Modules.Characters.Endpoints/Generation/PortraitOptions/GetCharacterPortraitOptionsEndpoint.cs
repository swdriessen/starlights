using FastEndpoints;
using Starlights.Modules.Characters.Domain;

namespace Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

public sealed class GetCharacterPortraitOptionsEndpoint : EndpointWithoutRequest<GetCharacterPortraitOptionsResponse>
{
    public override void Configure()
    {
        Get("/portrait-options");
        AllowAnonymous();
        Group<CharactersGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        // Generate random portrait URLs using Lorem Picsum service
        var random = new Random();
        var portraitUrls = new List<CharacterPortraitOption>();

        for (int i = 0; i < 25; i++) // Generate 25 random portraits
        {
            var width = 400;
            var height = 400;
            var seed = random.Next(1, 1000); // Random seed for variety

            var portraitOption = new CharacterPortraitOption
            {
                Url = $"https://picsum.photos/seed/{seed}/{width}/{height}",
                Description = $"Portrait {i + 1}"
            };

            portraitUrls.Add(portraitOption);
        }

        var response = new GetCharacterPortraitOptionsResponse
        {
            Portraits = portraitUrls
        };

        await Send.OkAsync(response, ct);
    }
}

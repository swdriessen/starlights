using FastEndpoints;
using Starlights.Modules.Characters.Domain;

namespace Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

public sealed class GetCharacterPortraitOptionsEndpoint : EndpointWithoutRequest<GetCharacterPortraitOptionsResponse>
{
    public override void Configure()
    {
        Get("/portrait-options");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        // Generate random portrait URLs using Lorem Picsum service
        var random = new Random();
        var portraitUrls = new List<CharacterPortraitOption>
        {
            new() { Url = "/portraits/portrait-1.jpg" },
            new() { Url = "/portraits/portrait-2.jpg" },
            new() { Url = "/portraits/portrait-3.jpg" },
            new() { Url = "/portraits/portrait-4.jpg" },
            new() { Url = "/portraits/portrait-5.jpg" },
            new() { Url = "/portraits/portrait-6.jpg" },
            new() { Url = "/portraits/portrait-7.jpg" },
            new() { Url = "/portraits/portrait-8.jpg" },
            new() { Url = "/portraits/portrait-9.jpg" },
            new() { Url = "/portraits/portrait-10.png" },
            new() { Url = "/portraits/portrait-11.png" },
            new() { Url = "/portraits/portrait-12.png" },
            new() { Url = "/portraits/portrait-13.png" },
            new() { Url = "/portraits/portrait-14.png" },
            new() { Url = "/portraits/portrait-15.png" },
            new() { Url = "/portraits/portrait-16.jpg" },
            new() { Url = "/portraits/portrait-17.jpg" },
            new() { Url = "/portraits/portrait-18.png" },
            new() { Url = "/portraits/portrait-19.png" },
            new() { Url = "/portraits/portrait-20.jpg" },
        };

        //for (int i = 0; i < 25; i++) // Generate 25 random portraits
        //{
        //    var width = 400;
        //    var height = 400;
        //    var seed = random.Next(1, 1000); // Random seed for variety

        //    var portraitOption = new CharacterPortraitOption
        //    {
        //        Url = $"https://picsum.photos/seed/{seed}/{width}/{height}",
        //        Description = $"Portrait {i + 1}"
        //    };

        //    portraitUrls.Add(portraitOption);
        //}

        var response = new GetCharacterPortraitOptionsResponse
        {
            Portraits = portraitUrls
        };

        await Send.OkAsync(response, ct);
    }
}

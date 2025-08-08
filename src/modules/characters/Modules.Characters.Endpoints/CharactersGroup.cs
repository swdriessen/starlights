using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Starlights.Modules.Characters.Endpoints;

public class CharactersGroup : Group
{
    public CharactersGroup()
    {
        base.Configure("characters", ep =>
        {
            ep.Description(d => d.WithTags("Characters"));
            ep.Options(o => o.RequireCors());
        });
    }
}

using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Starlights.Modules.Characters.Endpoints;

public class CharacterBuilderGroup : Group
{
    public CharacterBuilderGroup()
    {
        base.Configure("characters/{characterId:guid}/builder", ep =>
        {
            ep.Description(d => d.WithTags("Characters Builder", "Builder"));
            ep.Options(o => o.RequireCors());
        });
    }
}
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Starlights.Modules.Characters.Endpoints;

public class CharacterSheetGroup : Group
{
    public CharacterSheetGroup()
    {
        base.Configure("characters/{characterId:guid}/sheet", ep =>
        {
            ep.Description(d => d.WithTags("Characters Sheet"));
            ep.Options(o => o.RequireCors());
        });
    }
}

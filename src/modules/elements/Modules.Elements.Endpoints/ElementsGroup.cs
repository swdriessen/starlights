using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Starlights.Modules.Elements.Endpoints;

public class ElementsGroup : Group
{
    public ElementsGroup()
    {
        base.Configure("elements", ep =>
        {
            ep.Description(d => d.WithTags("Elements"));
            ep.Options(o => o.RequireCors());
        });
    }
}

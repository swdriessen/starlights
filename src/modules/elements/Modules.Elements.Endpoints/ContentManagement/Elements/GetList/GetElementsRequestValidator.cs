using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.GetList;

public class GetElementsRequestValidator : Validator<GetElementsRequest>
{
    public GetElementsRequestValidator()
    {
        RuleFor(x => x.Type)
            .MaximumLength(200)
            .When(x => x.Type is not null);
    }
}

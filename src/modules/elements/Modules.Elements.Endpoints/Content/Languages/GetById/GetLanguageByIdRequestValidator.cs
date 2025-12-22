using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Languages.GetById;

public sealed class GetLanguageByIdRequestValidator : Validator<GetLanguageByIdRequest>
{
    public GetLanguageByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}

using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Create;

public class CreateLanguageRequestValidator : Validator<CreateLanguageRequest>
{
    public CreateLanguageRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Kind).NotEmpty().WithMessage("Kind is required.");
    }
}

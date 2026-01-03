using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Update;

public class UpdateElementRequestValidator : Validator<UpdateElementRequest>
{
    public UpdateElementRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}

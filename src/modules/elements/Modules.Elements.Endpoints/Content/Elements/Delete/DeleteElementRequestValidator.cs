using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Delete;

public class DeleteElementRequestValidator : Validator<DeleteElementRequest>
{
    public DeleteElementRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}

using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.GetById;

public class GetElementByIdRequestValidator : Validator<GetElementByIdRequest>
{
    public GetElementByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}

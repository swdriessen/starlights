using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.GetById;

public sealed class GetSubClassByIdRequestValidator : Validator<GetSubClassByIdRequest>
{
    public GetSubClassByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

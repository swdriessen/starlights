using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.GetById;

public sealed class GetClassByIdRequestValidator : Validator<GetClassByIdRequest>
{
    public GetClassByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

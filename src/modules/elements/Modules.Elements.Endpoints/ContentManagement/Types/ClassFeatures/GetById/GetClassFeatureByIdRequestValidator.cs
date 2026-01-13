using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.GetById;

public sealed class GetClassFeatureByIdRequestValidator : Validator<GetClassFeatureByIdRequest>
{
    public GetClassFeatureByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

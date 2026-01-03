using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Delete;

public sealed class DeleteIncludeRuleRequestValidator : Validator<DeleteIncludeRuleRequest>
{
    public DeleteIncludeRuleRequestValidator()
    {
        RuleFor(x => x.ElementId).NotEmpty().WithMessage("ElementId is required.");
        RuleFor(x => x.RuleId).NotEmpty().WithMessage("RuleId is required.");
    }
}

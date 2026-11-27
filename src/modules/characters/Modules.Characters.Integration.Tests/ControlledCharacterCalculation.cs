using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Endpoints.Building;
using Starlights.RuleSets.DND5E5.Building;

namespace Starlights.Modules.Characters.Integration.Tests;

internal sealed class ControlledCharacterCalculation(bool fail = false) : ICharacterSheetCalculation
{
    public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public async Task<BuildView> CalculateAsync(BuildRules rules, ICharacterComposition composition)
    {
        Started.TrySetResult();
        await Release.Task;
        if (fail) throw new InvalidOperationException("Private internal failure should not reach clients.");
        return rules.View(composition);
    }
}

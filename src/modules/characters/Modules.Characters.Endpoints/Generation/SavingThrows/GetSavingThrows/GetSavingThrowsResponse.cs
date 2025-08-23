using Starlights.Modules.Characters.Endpoints.Generation.SavingThrows;

namespace Starlights.Modules.Characters.Endpoints.Generation.SavingThrows.GetSavingThrows;

internal sealed class GetSavingThrowsResponse
{
    public List<SavingThrowDataModel> SavingThrows { get; set; } = [];
}

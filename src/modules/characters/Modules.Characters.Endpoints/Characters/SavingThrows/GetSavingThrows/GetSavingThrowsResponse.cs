using Starlights.Modules.Characters.Endpoints.Characters.SavingThrows;

namespace Starlights.Modules.Characters.Endpoints.Characters.SavingThrows.GetSavingThrows;

internal sealed class GetSavingThrowsResponse
{
    public List<SavingThrowDataModel> SavingThrows { get; set; } = [];
}

using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;

namespace Starlights.Modules.Characters.Endpoints.CharacterSheet.GetFeatures;

public sealed class GetFeaturesResponse
{
    public List<RegistrationDataModel> Features { get; set; } = [];
}

using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Abilities;

public sealed class AbilityScore : EntityBase<AbilityScoreId>
{
    private AbilityScore(RegistrationId associatedRegistrationId, string name, string abbreviation)
        : base(AbilityScoreId.New())
    {
        AssociatedRegistrationId = associatedRegistrationId;
        Name = name;
        Abbreviation = abbreviation;
    }

    public RegistrationId AssociatedRegistrationId { get; }
    public string Name { get; }
    public string Abbreviation { get; }
    public int BaseScore { get; set; } = 10;

    public static AbilityScore Create(RegistrationId associatedRegistrationId, string name, string abbreviation)
    {
        return new AbilityScore(associatedRegistrationId, name, abbreviation);
    }
}

using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Provides context for statistics processors.
/// </summary>
public sealed class StatisticsProcessorContext
{
    public StatisticsProcessorContext(Character character, List<Registration> registrations)
    {
        Character = character;
        Registrations = registrations;
        RegistrationLookup = registrations.ToDictionary(r => r.Id, r => r.AssociatedElementName);
    }

    /// <summary>
    /// Gets the character for whom statistics are being processed.
    /// </summary>
    public Character Character { get; }

    /// <summary>
    /// Gets all registrations relevant to the character from which statistics can be derived.
    /// </summary>
    public IReadOnlyCollection<Registration> Registrations { get; }

    /// <summary>
    /// The statistics collection being built, modified, and processed.
    /// </summary>
    public StatisticValuesGroupCollection Statistics { get; } = [];

    /// <summary>
    /// A lookup dictionary mapping registration IDs to their associated element names use for display purpose.
    /// </summary>
    public IReadOnlyDictionary<RegistrationId, string> RegistrationLookup { get; }
}

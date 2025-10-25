using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Provides context for statistics processors.
/// </summary>
public sealed class StatisticsProcessorContext
{
    private readonly List<string> _errors = [];

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

    /// <summary>
    /// Gets a read-only list of error messages.
    /// </summary>
    public IReadOnlyList<string> Errors => _errors;

    /// <summary>
    /// Adds an error message to the collection of errors.
    /// </summary>
    public void AddError(string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(errorMessage);
        _errors.Add(errorMessage);
    }

    /// <summary>
    /// Gets a value indicating whether any errors are present.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;
}

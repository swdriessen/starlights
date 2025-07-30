namespace Starlights.Modules.Elements.Integration.Abstractions;

public interface IElementsModuleInitialization
{
    /// <summary>
    /// Initializes the elements module, typically by creating default elements (based on game system) or performing necessary setup.
    /// </summary>
    /// <remarks>
    /// This may also be used for testing purposes by injecting a different IElementsModuleInitialization in the container.
    /// </remarks>
    Task<InitializationResult> InitializeAsync();
}

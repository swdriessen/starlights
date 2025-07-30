namespace Starlights.Modules.Elements.Integration.Abstractions;

public interface IElementsModuleInitializer
{
    /// <summary>
    /// Initializes the elements module, typically by creating default elements (based on game system) or performing necessary setup.
    /// </summary>
    /// <remarks>
    /// This may also be used for testing purposes by injecting a different IElementsModuleInitializer in the container.
    /// </remarks>
    Task<InitializationResult> InitializeAsync();
}

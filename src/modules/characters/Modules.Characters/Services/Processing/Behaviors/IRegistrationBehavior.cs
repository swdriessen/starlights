using Starlights.Modules.Characters.Domain.Registrations;

namespace Modules.Characters.Services.Processing.Behaviors;

public interface IRegistrationBehavior
{
    /// <summary>
    /// This method is called when a new registration is created.
    /// </summary>
    /// <param name="newRegistration">The registration that was just created. Note: at this time the changes have not been saved to the characters database yet.</param>
    /// <param name="context">Additional context for the current registration that is being processed.</param>
    Task Registered(Registration newRegistration, RegistrationProcessContext context);
}

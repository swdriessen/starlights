using System;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Processing;

public interface INewRegistrationManager
{
    Task Register(Registration newRegistration, RegistrationProcessContext context);

    Task Unregister(Registration existingRegistration);
}
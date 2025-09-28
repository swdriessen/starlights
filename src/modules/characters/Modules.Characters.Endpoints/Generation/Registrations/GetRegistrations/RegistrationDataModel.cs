namespace Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;

sealed class RegistrationDataModel
{
    public required Guid RegistrationId { get; set; }
    public required Guid CharacterId { get; set; }
    public required Guid AssociatedElementId { get; set; }
    public required Guid? ParentRegistrationId { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }

    public List<RegistrationDataModel> Children { get; set; } = [];
}


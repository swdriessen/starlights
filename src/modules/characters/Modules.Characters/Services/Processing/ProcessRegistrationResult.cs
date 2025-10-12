namespace Starlights.Modules.Characters.Services.Processing;

public class ProcessRegistrationResult
{
    public int AffectedRows { get; set; }

    public string? ErrorMessage { get; init; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public static ProcessRegistrationResult Success(int affectedRows = 0)
    {
        return new ProcessRegistrationResult { AffectedRows = affectedRows };
    }

    public static ProcessRegistrationResult Failure(string errorMessage)
    {
        return new ProcessRegistrationResult { ErrorMessage = errorMessage };
    }
}

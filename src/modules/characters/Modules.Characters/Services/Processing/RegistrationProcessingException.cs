namespace Starlights.Modules.Characters.Services.Processing;

[Serializable]
public class RegistrationProcessingException : Exception
{
    public RegistrationProcessingException() { }
    public RegistrationProcessingException(string message) : base(message) { }
    public RegistrationProcessingException(string message, Exception inner) : base(message, inner) { }
    protected RegistrationProcessingException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
using System.Runtime.Serialization;

namespace Starlights.Platform.Hosting;

[Serializable]
public class PlatformException : Exception
{
    public PlatformException() { }
    public PlatformException(string message) : base(message) { }
    public PlatformException(string message, Exception inner) : base(message, inner) { }
    protected PlatformException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

[Serializable]
public class PlatformModuleRegistrationException : PlatformException
{
    public PlatformModuleRegistrationException() { }
    public PlatformModuleRegistrationException(string message) : base(message) { }
    public PlatformModuleRegistrationException(string message, Exception inner) : base(message, inner) { }
    protected PlatformModuleRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

[Serializable]
public class PlatformComponentRegistrationException : PlatformException
{
    public PlatformComponentRegistrationException() { }
    public PlatformComponentRegistrationException(string message) : base(message) { }
    public PlatformComponentRegistrationException(string message, Exception inner) : base(message, inner) { }
    protected PlatformComponentRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
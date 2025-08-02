namespace Starlights.Platform.Hosting;

[Serializable]
public class PlatformException : Exception
{
    public PlatformException() { }
    public PlatformException(string message) : base(message) { }
    public PlatformException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class PlatformModuleRegistrationException : PlatformException
{
    public PlatformModuleRegistrationException() { }
    public PlatformModuleRegistrationException(string message) : base(message) { }
    public PlatformModuleRegistrationException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class PlatformComponentRegistrationException : PlatformException
{
    public PlatformComponentRegistrationException() { }
    public PlatformComponentRegistrationException(string message) : base(message) { }
    public PlatformComponentRegistrationException(string message, Exception inner) : base(message, inner) { }
}
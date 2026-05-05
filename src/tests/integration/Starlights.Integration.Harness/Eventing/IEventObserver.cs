namespace Starlights.Integration.Eventing;

internal interface IEventObserver
{
    /// <summary>
    /// Clears all recorded invocations of the observer, resetting its state to be ready for new events.
    /// </summary>
    void ClearInvocations();
}

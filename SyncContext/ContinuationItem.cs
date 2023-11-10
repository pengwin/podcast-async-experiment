namespace Async.SyncContext;

readonly struct ContinuationItem
{
    public readonly SendOrPostCallback Continuation;
    public readonly object? State;

    public ContinuationItem(SendOrPostCallback continuation, object? state)
    {
        Continuation = continuation;
        State = state;
    }

    public void Deconstruct(out SendOrPostCallback continuation, out object? state)
    {
        continuation = Continuation;
        state = State;
    }
}
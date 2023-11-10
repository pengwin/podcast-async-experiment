using System.Collections.Concurrent;

namespace Async.SyncContext;

public class SingleThreadedContext : SynchronizationContext, IDisposable
{
    private readonly BlockingCollection<ContinuationItem> _queue = new();
    private readonly Thread _mainThread;

    public SingleThreadedContext()
    {
        _mainThread = new Thread(MainThread);
        _mainThread.Start();
    }

    /// <summary>
    /// Dispatches an asynchronous message to a synchronization context.
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="state"></param>
    public override void Post(SendOrPostCallback callback, object? state)
    {
        _queue.Add(new ContinuationItem(callback, state));
        Log($"Post called {_queue.Count}");
    }

    /// <summary>
    /// Dispatches a synchronous message to a synchronization context
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="state"></param>
    public override void Send(SendOrPostCallback callback, object? state)
    {
        Log($"Send started");
        using var mre = new ManualResetEvent(false);
        SendOrPostCallback continuation = s =>
        {
            callback(s);
            mre.Set();
        };
        var item = new ContinuationItem(continuation, state);
        _queue.Add(item);
        mre.WaitOne();
        Log($"Send ended");
    }

    public override SynchronizationContext CreateCopy() => this;

    public void Dispose()
    {
        _queue.CompleteAdding();
        _mainThread.Join(1000);
        Log($"Context disposed");
    }

    private void MainThread()
    {
        SetSynchronizationContext(this);
        Log($"Main ThreadId {Thread.CurrentThread.ManagedThreadId}");
        foreach (var (continuation, state) in _queue.GetConsumingEnumerable())
        {
            Log($"Executing continuation");
            continuation(state);
        }
    }

    private void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Context: {0}", message);
        Console.ResetColor();
    }
}
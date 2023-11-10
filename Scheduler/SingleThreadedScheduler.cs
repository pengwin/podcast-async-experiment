
using System.Collections.Concurrent;
using Async.SyncContext;

namespace Async.Scheduler;

public class SingleThreadedScheduler : TaskScheduler, IDisposable
{
    private readonly BlockingCollection<Task> _queue = new();
    private readonly Thread _mainThread;
    private readonly SingleThreadedContext? _context;

    public SingleThreadedScheduler(SingleThreadedContext? context)
    {
        _context = context;
        _mainThread = new Thread(MainThread);
        _mainThread.Start();
    }

    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        return _queue.ToArray();
    }

    protected override void QueueTask(Task task)
    {
        Log($"Task {task.Id} queued");
        _queue.Add(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }

    public void Dispose()
    {
        _queue.CompleteAdding();
        _mainThread.Join(1000);
        Log($"disposed");
    }

    private void MainThread()
    {
        if (_context != null)
        {
            SynchronizationContext.SetSynchronizationContext(_context);
        }
        Log($"Main ThreadId {Thread.CurrentThread.ManagedThreadId}");
        foreach (var task in _queue.GetConsumingEnumerable())
        {
            Log($"Executing task {task.Id}");
            TryExecuteTask(task);
        }
    }

    private void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Scheduler: {0}", message);
        Console.ResetColor();
    }
}
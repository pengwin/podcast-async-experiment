namespace Async.Examples;

public static class DeadlockExample
{
    private static SemaphoreSlim _lockObject = new SemaphoreSlim(1, 1);

    public static void Run(TaskScheduler scheduler, bool continueOnCapturedContext)
    {
        var tasks = new [] {
            TaskRunner.RunTask(() => RunBlockedTask(continueOnCapturedContext), scheduler),
            TaskRunner.RunTask(() => RunBlockedTask(continueOnCapturedContext), scheduler),
        };

        Task.WaitAll(tasks);
    }

    private async static Task RunBlockedTask(bool continueOnCapturedContext)
    {
        _lockObject.Wait();
        Console.WriteLine("Lock acquired on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
        await YieldTask().ConfigureAwait(continueOnCapturedContext);
        _lockObject.Release();
        Console.WriteLine("Lock released on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
    }

    public static async Task YieldTask()
    {
        await Task.Yield();
    }
}
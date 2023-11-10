namespace Async.Examples;

public static class ConfigureAwaitExample
{
    public static void Run(TaskScheduler scheduler)
    {
        var task = TaskRunner.RunTask(async () =>
        {
            Console.WriteLine("Task started  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
                await YieldTask().ConfigureAwait(false);
                Console.WriteLine("Task continuation1 on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
                await YieldTask().ConfigureAwait(false);
                Console.WriteLine("Task continuation2 on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
        }, scheduler);

        task.Wait();
    }

    public static async Task YieldTask()
    {
        await Task.Yield();
    }
}
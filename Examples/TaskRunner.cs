namespace Async.Examples;

public static class TaskRunner
{
    public static Task RunTask(Func<Task> action, TaskScheduler scheduler)
    {
        return Task.Factory.StartNew(
            action,
            CancellationToken.None,
            TaskCreationOptions.None,
            scheduler);
    }

    public static Task CreateTask(string taskName, TaskScheduler scheduler)
    {
        return RunTask(
            async () =>
            {
                Console.WriteLine("Task {0} started  on ThreadId {1}", taskName, Thread.CurrentThread.ManagedThreadId);
                await Task.Yield();
                Console.WriteLine("Task {0} continuation1 on ThreadId {1}", taskName, Thread.CurrentThread.ManagedThreadId);
                await Task.Yield();
                Console.WriteLine("Task {0} continuation2 on ThreadId {1}", taskName, Thread.CurrentThread.ManagedThreadId);
            },
            scheduler);
    }
}
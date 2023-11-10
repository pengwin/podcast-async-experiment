namespace Async.Examples;

public static class SeveralTasksExample
{
    public static void Run(TaskScheduler scheduler)
    {
        var tasks = new[]
        {
            CreateTask("1", scheduler),
            CreateTask("2", scheduler),
            CreateTask("3", scheduler),
        };

        Task.WaitAll(tasks);
    }

    public static Task CreateTask(string taskName, TaskScheduler scheduler)
    {
        return TaskRunner.RunTask(
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
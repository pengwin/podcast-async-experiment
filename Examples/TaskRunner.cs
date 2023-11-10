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
}
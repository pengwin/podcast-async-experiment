namespace Async.Examples;

public static class SeveralTasksExample
{
    public static void Run(TaskScheduler scheduler)
    {
        var tasks = new[]
        {
            TaskRunner.CreateTask("1", scheduler),
            TaskRunner.CreateTask("2", scheduler),
            TaskRunner.CreateTask("3", scheduler),
        };

        Task.WaitAll(tasks);
    }
}
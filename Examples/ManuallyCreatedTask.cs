using Async.CustomAsync;

namespace Async.Examples;

public static class ManuallyCreatedTask
{
    public static void Run(TaskScheduler scheduler)
    {
        var task = TaskRunner.RunTask(CustomTaskBuilder.BuildTask, scheduler);
        task.Wait();
    }
}
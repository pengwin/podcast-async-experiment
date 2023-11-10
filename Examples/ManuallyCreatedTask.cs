using Async.CustomAsync;

namespace Async.Examples;

public static class ManuallyCreatedTask
{
    public static void Run(TaskScheduler scheduler)
    {
        var task = Task.Factory.StartNew(
            CustomTaskBuilder.BuildTask,
            CancellationToken.None,
            TaskCreationOptions.None,
            scheduler);

        task.Wait();
    }
}
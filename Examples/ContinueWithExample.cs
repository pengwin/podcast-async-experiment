namespace Async.Examples;

public static class ContinueWithExample
{
    public static void Run(TaskScheduler scheduler)
    {
        var initialTask = TaskRunner.RunTask(() =>
        {
            Console.WriteLine("InitialTask on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
            return Task.CompletedTask;
        }, scheduler);

        var task = initialTask.ContinueWith(
            t => Console.WriteLine("ContinueWith1 on ThreadId {0}", Thread.CurrentThread.ManagedThreadId),
            CancellationToken.None,
            TaskContinuationOptions.AttachedToParent,
            scheduler).ContinueWith(
            t => Console.WriteLine("ContinueWith2 on ThreadId {0}", Thread.CurrentThread.ManagedThreadId),
            CancellationToken.None,
            TaskContinuationOptions.AttachedToParent,
            scheduler);

        task.Wait();
    }
}
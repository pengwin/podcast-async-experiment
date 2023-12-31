﻿using Async.Scheduler;
using Async.SyncContext;
using Async.Examples;

var useSyncContext = ReadSyncOrWithoutSync();
var exampleNum = ReadExampleNum();

try 
{
    using var context = new SingleThreadedContext();
    SynchronizationContext.SetSynchronizationContext(context);
    using var scheduler = new SingleThreadedScheduler(useSyncContext ? context : null);

    switch (exampleNum)
    {
        case 1:
            ContinueWithExample.Run(scheduler);
            break;
        case 2:
            ManuallyCreatedTask.Run(scheduler);
            break;
        case 3:
            SeveralTasksExample.Run(scheduler);
            break;
        case 4:
            ConfigureAwaitExample.Run(scheduler);
            break;
        case 5:
            DeadlockExample.Run(scheduler, true);
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

static bool ReadSyncOrWithoutSync()
{
    bool? result = null;
    while (!result.HasValue)
    {
        Console.WriteLine("Use sync context in scheduler (Y/N)");
        var input = Console.ReadLine()?.ToUpperInvariant();
        if (input == "Y")
        {
            result = true;
        }
        else if (input == "N")
        {
            result = false;
        }
    }
    
    return result.Value;
}

static int ReadExampleNum()
{
    int? result = null;
    while (!result.HasValue)
    {
        Console.WriteLine("Choose example to run:");
        Console.WriteLine("\t 1) Run task with ContinueWith");
        Console.WriteLine("\t 2) Run manually created task");
        Console.WriteLine("\t 3) Run several tasks");
        Console.WriteLine("\t 4) Run task with configure await false");
        Console.WriteLine("\t 5) Run task with deadlock configure await false");
        var input = Console.ReadLine();
        switch (input)
        {
            case "1":
                result = 1;
                break;
            case "2":
                result = 2;
                break;
            case "3":
                result = 3;
                break;
            case "4":
                result = 4;
                break;
            case "5":
                result = 5;
                break;
        }
    }
    
    return result.Value;
}
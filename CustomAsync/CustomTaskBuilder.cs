

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Async.CustomAsync;

public enum StateMachineState
{
    Initial,
    Continuation1,
    Continuation2,
    Completed,

}

public sealed class StateMachine : IAsyncStateMachine
{
    private readonly AsyncTaskMethodBuilder _builder;
    private StateMachineState _state = StateMachineState.Initial;

    public StateMachine(AsyncTaskMethodBuilder builder)
    {
        _builder = builder;
    }

    public void MoveNext()
    {
        if (_state == StateMachineState.Completed)
        {
            return;
        }
        _state = SwitchState();
        if (_state == StateMachineState.Completed)
        {
            _builder.SetResult();
            return;
        }
        YieldTask();
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        _builder.SetStateMachine(stateMachine);
    }

    private void YieldTask() 
    {
        YieldAwaitable.YieldAwaiter awaiter = Task.Yield().GetAwaiter();
        if (!awaiter.IsCompleted)
        {
            var stateMachine = this;
            _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }
    }

    private StateMachineState SwitchState() => _state switch
    {
        StateMachineState.Initial => Initial(),
        StateMachineState.Continuation1 => Continuation1(),
        StateMachineState.Continuation2 => Continuation2(),
        StateMachineState.Completed => StateMachineState.Completed,
        _ => throw new InvalidOperationException($"Unknown state {_state}")
    };

    private StateMachineState Initial()
    {
        Console.WriteLine("Initial  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
        return StateMachineState.Continuation1;
    }

    private StateMachineState Continuation1()
    {
        Console.WriteLine("Continuation1  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
        return StateMachineState.Continuation2;
    }

    private StateMachineState Continuation2()
    {
        Console.WriteLine("Continuation2  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
        return StateMachineState.Completed;
    }
}

public static class CustomTaskBuilder
{
    public static Task BuildTask()
    {
        var builder = AsyncTaskMethodBuilder.Create();
        var machine = new StateMachine(builder);
        builder.Start(ref machine);
        return builder.Task;
    }
}
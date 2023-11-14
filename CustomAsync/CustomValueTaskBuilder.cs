using System.Runtime.CompilerServices;

namespace Async.CustomAsync;

public enum ValueTaskStateMachineState
{
    Initial,
    Continuation1,
    Continuation2,
    Completed,

}

public sealed class ValueTaskStateMachine : IAsyncStateMachine
{
    private readonly AsyncValueTaskMethodBuilder _builder;
    private StateMachineState _state = StateMachineState.Initial;

    public ValueTaskStateMachine(AsyncValueTaskMethodBuilder builder)
    {
        _builder = builder;
    }

    public void MoveNext() 
    {
        switch (_state)
        {
            case StateMachineState.Initial:
                Initial();
                _state = StateMachineState.Continuation1;
                YieldTask();
                return;
            case StateMachineState.Continuation1:
                Continuation1();
                _state = StateMachineState.Continuation2;
                YieldTask();
                return;
            case StateMachineState.Continuation2:
                Continuation2();
                _state = StateMachineState.Completed;
                YieldTask();
                return;
            case StateMachineState.Completed:
                _builder.SetResult();
                return;
            default:
                throw new InvalidOperationException($"Unknown state {_state}");
        
        }

        void YieldTask()
        {
            YieldAwaitable.YieldAwaiter awaiter = Task.Yield().GetAwaiter();
            var stateMachine = this;
            _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        _builder.SetStateMachine(stateMachine);
    }

    private void Initial()
    {
        Console.WriteLine("Initial  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
    }

    private void Continuation1()
    {
        Console.WriteLine("Continuation1  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
    }

    private void Continuation2()
    {
        Console.WriteLine("Continuation2  on ThreadId {0}", Thread.CurrentThread.ManagedThreadId);
    }
}

public static class CustomValueTaskBuilder
{
    public static ValueTask BuildTask()
    {
        var builder = AsyncValueTaskMethodBuilder.Create();
        var machine = new ValueTaskStateMachine(builder);
        builder.Start(ref machine);
        return builder.Task;
    }
}
namespace DoliteTemplate.Shared.Utils;

public class Monad
{
    internal Monad(Exception? exception = null)
    {
        Exception = exception;
    }

    public Exception? Exception { get; protected init; }
    public bool Success => Exception is null;

    public override string ToString()
    {
        return Success ? "Monad.Success" : $"Monad.Failure{{{Exception}}}";
    }

    public static Monad Ok()
    {
        return new Monad();
    }

    public static Monad Ok<TResult>(TResult result)
    {
        return new Monad<TResult>(result);
    }

    public static Monad Err(Exception exception)
    {
        return new Monad(exception);
    }

    public static Monad Err(string errorMessage)
    {
        return new Monad(new Exception(errorMessage));
    }

    public bool TryGetValue<TResult>(out TResult? result)
    {
        result = default;
        if (!Success)
        {
            return false;
        }

        if (this is not IMonadWithResult<TResult> monad)
        {
            return false;
        }

        result = monad.Result;
        return true;
    }

    public bool TryGetValue(Type type, out object? result)
    {
        result = default;
        if (!Success)
        {
            return false;
        }

        if (this is not IMonadWithResult monad)
        {
            return false;
        }

        result = monad.Result;
        return result.GetType() == type;
    }

    public bool TryGetValue(out object? result)
    {
        result = default;
        if (!Success)
        {
            return false;
        }

        if (this is not IMonadWithResult monad)
        {
            return false;
        }

        result = monad.Result;
        return true;
    }

    public static Monad Handle(Action task)
    {
        try
        {
            task();
            return new Monad();
        }
        catch (Exception e)
        {
            return new Monad(e);
        }
    }

    public static Monad<TResult> Handle<TResult>(Func<TResult> task)
    {
        try
        {
            var result = task();
            return new Monad<TResult>(result);
        }
        catch (Exception e)
        {
            return new Monad<TResult>(e);
        }
    }
}

public class Monad<TResult> : Monad, IMonadWithResult<TResult>
{
    private readonly TResult? _result;

    internal Monad(TResult result)
    {
        _result = result;
    }

    internal Monad(Exception exception)
    {
        Exception = exception;
    }

    public TResult Result => _result ?? throw Exception ?? new Exception("unknown");
    object IMonadWithResult.Result => Result ?? throw Exception ?? new Exception("unknown");

    public override string ToString()
    {
        return Success ? $"Monad.Success{{{Result}}}" : $"Monad.Failure{{{Exception?.Message}}}";
    }
}

public interface IMonadWithResult
{
    object Result { get; }
}

public interface IMonadWithResult<out TResult> : IMonadWithResult
{
    new TResult Result { get; }
}
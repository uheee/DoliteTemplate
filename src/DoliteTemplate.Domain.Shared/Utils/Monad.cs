namespace DoliteTemplate.Domain.Shared.Utils;

/// <summary>
///     无内容结果包装器
/// </summary>
public class Monad
{
    internal Monad(Exception? exception = null)
    {
        Exception = exception;
    }

    /// <summary>
    ///     异常
    ///     <para>成功时为null</para>
    /// </summary>
    public Exception? Exception { get; protected init; }

    /// <summary>
    ///     是否成功
    /// </summary>
    public bool Success => Exception is null;

    public override string ToString()
    {
        return Success ? "Monad.Success" : $"Monad.Failure{{{Exception}}}";
    }

    /// <summary>
    ///     成功结果
    /// </summary>
    /// <returns></returns>
    public static Monad Ok()
    {
        return new Monad();
    }

    /// <summary>
    ///     成功结果
    /// </summary>
    /// <param name="result">结果内容</param>
    /// <typeparam name="TResult">结果内容类型</typeparam>
    /// <returns></returns>
    public static Monad Ok<TResult>(TResult result)
    {
        return new Monad<TResult>(result);
    }

    /// <summary>
    ///     错误结果
    /// </summary>
    /// <param name="exception">错误异常</param>
    /// <returns></returns>
    public static Monad Err(Exception exception)
    {
        return new Monad(exception);
    }

    /// <summary>
    ///     错误结果
    /// </summary>
    /// <param name="errorMessage">错误消息</param>
    /// <returns></returns>
    public static Monad Err(string errorMessage)
    {
        return new Monad(new Exception(errorMessage));
    }

    /// <summary>
    ///     尝试获取成功结果内容
    /// </summary>
    /// <param name="result">结果内容，仅在结果成功且存在<b>与指定类型一致</b>的结果内容时返回非null值</param>
    /// <typeparam name="TResult">结果内容类型</typeparam>
    /// <returns>仅在结果成功且存在<b>与指定类型一致</b>的结果内容时返回True</returns>
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

    /// <summary>
    ///     尝试获取成功结果内容
    /// </summary>
    /// <param name="type">结果内容类型</param>
    /// <param name="result">结果内容，仅在结果成功且存在<b>与指定类型一致</b>的结果内容时返回非null值</param>
    /// <returns>仅在结果成功且存在<b>与指定类型一致</b>的结果内容时返回True</returns>
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

    /// <summary>
    ///     尝试获取成功结果内容
    /// </summary>
    /// <param name="result">结果内容，仅在结果成功且存在结果内容时返回非null值</param>
    /// <returns>仅在结果成功且存在结果内容时返回True</returns>
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

    /// <summary>
    ///     包装同步过程
    /// </summary>
    /// <param name="task"><b>无返回值</b>的同步过程</param>
    /// <returns>包装器结果</returns>
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

    /// <summary>
    ///     包装同步过程
    /// </summary>
    /// <param name="task"><b>有返回值</b>的同步过程</param>
    /// <typeparam name="TResult">同步过程的返回值类型</typeparam>
    /// <returns>包装器结果</returns>
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

    /// <summary>
    ///     包装异步过程
    /// </summary>
    /// <param name="task"><b>无返回值</b>的异步过程</param>
    /// <returns>包装器结果</returns>
    public static async Task<Monad> Handle(Func<Task> task)
    {
        try
        {
            await task();
            return new Monad();
        }
        catch (Exception e)
        {
            return new Monad(e);
        }
    }

    /// <summary>
    ///     包装异步过程
    /// </summary>
    /// <param name="task"><b>有返回值</b>的异步过程</param>
    /// <typeparam name="TResult">异步过程的返回值类型</typeparam>
    /// <returns>包装器结果</returns>
    public static async Task<Monad<TResult>> Handle<TResult>(Func<Task<TResult>> task)
    {
        try
        {
            var result = await task();
            return new Monad<TResult>(result);
        }
        catch (Exception e)
        {
            return new Monad<TResult>(e);
        }
    }
}

/// <summary>
///     有内容结果包装器
/// </summary>
/// <typeparam name="TResult">内容类型</typeparam>
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

    /// <summary>
    ///     有类型结果内容
    /// </summary>
    /// <exception cref="Exception">当结果为错误时，返回错误异常</exception>
    public TResult Result => _result ?? throw Exception ?? new Exception("unknown");

    /// <summary>
    ///     无类型结果内容
    /// </summary>
    /// <exception cref="Exception">当结果为错误时，返回错误异常</exception>
    object IMonadWithResult.Result => Result ?? throw Exception ?? new Exception("unknown");

    public override string ToString()
    {
        return Success ? $"Monad.Success{{{Result}}}" : $"Monad.Failure{{{Exception?.Message}}}";
    }
}

/// <summary>
///     无类型内容特性
/// </summary>
public interface IMonadWithResult
{
    /// <summary>
    ///     无类型结果内容
    /// </summary>
    object Result { get; }
}

/// <summary>
///     有类型内容特性
/// </summary>
/// <typeparam name="TResult">结果内容类型</typeparam>
public interface IMonadWithResult<out TResult> : IMonadWithResult
{
    /// <summary>
    ///     有类型结果内容
    /// </summary>
    new TResult Result { get; }
}
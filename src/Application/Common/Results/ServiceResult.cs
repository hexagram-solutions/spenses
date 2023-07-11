namespace Spenses.Application.Common.Results;

public abstract record ServiceResult
{
    public abstract bool IsSuccess { get; }
}

public record ServiceResult<TValue> : IConvertToServiceResult
{
    public ServiceResult(ServiceResult result)
    {
        if (typeof(ServiceResult).IsAssignableFrom(typeof(TValue)))
            throw new ArgumentException("Cannot nest a service result within a service result.", nameof(result));

        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public ServiceResult(TValue value)
    {
        if (typeof(ServiceResult).IsAssignableFrom(typeof(TValue)))
            throw new ArgumentException("Cannot nest a service result within a service result.", nameof(value));

        Value = value;
        Result = new ValueResult(value);
    }

    public ServiceResult Result { get; set; }

    public TValue Value { get; set; } = default!;

    public bool IsSuccess => Result.IsSuccess;

    public static implicit operator ServiceResult<TValue>(ServiceResult result)
    {
        return new ServiceResult<TValue>(result);
    }

    public static implicit operator ServiceResult<TValue>(TValue value)
    {
        return new ServiceResult<TValue>(value);
    }

    private static Type[] GetValueType()
    {
        return new[]
        {
            typeof(TValue)
        };
    }

    public ServiceResult Convert()
    {
        return Result;
    }
}

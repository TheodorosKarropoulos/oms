namespace OMS.Domain.Common;

public class Result<T> : Result
{
    public T Value { get; private init; }

    public Result(
        bool successful,
        T value,
        string errorMessage,
        int statusCode,
        int eventId) : base(successful, errorMessage, statusCode, eventId)
    {
        Value = value;
    }
}
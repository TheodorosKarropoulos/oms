namespace OMS.Domain.Common;

public class Result
{
    public bool Successful { get; private init; }

    public string ErrorMessage { get; init; }

    public int StatusCode { get; protected init; }

    public int EventId { get; init; }

    protected Result(bool successful, string errorMessage, int statusCode, int eventId)
    {
        Successful = successful;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
        EventId = eventId;
    }

    public static Result Fail(string error, int statusCode)
    {
        return new Result(false, error, statusCode, 0);
    }

    public static Result Fail(string error, int statusCode, int eventId)
    {
        return new Result(false, error, statusCode, eventId);
    }
    
    public static Result<T> Fail<T>(string error, int statusCode)
    {
        return new Result<T>(false, default, error, statusCode, 0);
    }
    
    public static Result<T> Fail<T>(string error, int statusCode, int eventId)
    {
        return new Result<T>(false, default, error, statusCode, eventId);
    }

    public static Result Success()
    {
        return new Result(true, string.Empty, Common.StatusCode.Success, 0);
    }
    
    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(true, value, string.Empty, Common.StatusCode.Success, 0);
    }
    
    public static Result<T> Success<T>(T value, int statusCode)
    {
        return new Result<T>(true, value, string.Empty, statusCode, 0);
    }
}
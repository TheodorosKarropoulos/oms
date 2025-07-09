using OMS.Domain.Common;

namespace OMS.Api.Extensions;

internal static class ResultExtensions
{
    internal static IResult AsResult(this Result result)
    {
        if (result.Successful)
        {
            return Results.Ok();
        }

        var statusCode = result.StatusCode;

        var response = new
        {
            status = statusCode,
            message = result.ErrorMessage
        };

        return Results.Json(response, statusCode: statusCode);
    }

    internal static IResult AsResult<T>(this Result<T> result)
    {
        if (result.Successful)
        {
            return Results.Ok(result.Value);
        }
        
        var statusCode = result.StatusCode;

        var response = new
        {
            status = statusCode,
            message = result.ErrorMessage
        };

        return Results.Json(response, statusCode: statusCode);
    }
}
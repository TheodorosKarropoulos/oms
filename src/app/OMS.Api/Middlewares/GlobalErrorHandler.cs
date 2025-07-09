using System.ComponentModel.DataAnnotations;
using OMS.Domain.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace OMS.Api.Middlewares;

public class GlobalErrorHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandler> _logger;

    public GlobalErrorHandler(RequestDelegate next, ILogger<GlobalErrorHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (InvalidOrderStatusException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Unexpected error" });
            _logger.LogError(ex, "Unhandled exception");
        }
    }
}

public static class GlobalErrorHandlerExtensions
{
    public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalErrorHandler>();
    }
}
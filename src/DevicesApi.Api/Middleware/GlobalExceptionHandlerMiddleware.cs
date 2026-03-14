using DevicesApi.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace DevicesApi.Api.Middleware;

public sealed class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", 
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            DeviceNotFoundException ex       => (HttpStatusCode.NotFound, ex.Message),
            DeviceInUseException ex          => (HttpStatusCode.Conflict, ex.Message),
            DeviceDeletionNotAllowedException ex => (HttpStatusCode.Conflict, ex.Message),
            _                                => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = statusCode.ToString(),
            message
        });

        return context.Response.WriteAsync(body);
    }
}

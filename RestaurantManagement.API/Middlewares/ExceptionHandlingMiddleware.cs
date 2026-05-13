using FluentValidation;
using RestaurantManagement.Application.Common.Exeption;
using System.Text.Json;

namespace RestaurantManagement.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new { message = "", errors = new List<string>() };

        switch (exception)
        {
            case ApiException apiEx:
                context.Response.StatusCode = apiEx.StatusCode;
                response = new { message = apiEx.Message, errors = new List<string>() };
                break;

            case ValidationException validationEx:
                context.Response.StatusCode = 400;
                response = new
                {
                    message = "Validation failed",
                    errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList()
                };
                break;

            default:
                context.Response.StatusCode = 500;
                response = new { message = "Internal server error", errors = new List<string>() };
                logger.LogError(exception, "Unhandled exception");
                break;
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
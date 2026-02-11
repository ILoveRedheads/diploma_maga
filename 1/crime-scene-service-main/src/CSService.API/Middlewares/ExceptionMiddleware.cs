using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CSService.Common.Exceptions;
using CSService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSService.API.Middlewares;

internal class ExceptionMiddleware(
    ILogger<ExceptionMiddleware> logger,
    RequestDelegate next,
    IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception ex) {
            switch (ex) {
                case ArgumentException:
                    await HandleExceptionAsync(ex.Message, context, ex, HttpStatusCode.BadRequest);
                    break;

                case BaseException bex:
                    await HandleExceptionAsync(bex.Message, context, bex, bex.StatusCode);
                    break;

                default:
                    await HandleExceptionAsync(
                        "Internal server error",
                        context,
                        ex,
                        HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }

    private Task HandleExceptionAsync(string title, HttpContext context, Exception ex, HttpStatusCode code) {
        var message = $"{code}: {ex.Message}";
        _logger.LogError(ex, message, ex.StackTrace);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var problemDetails = new ErrorDetails {
            Title = title,
            Detail = _configuration.GetSection("AllowFullError").Get<bool>() ? ex.ToString() : null,
            Status = (int)code,
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(
            problemDetails,
            options: new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}

using FluentValidation;
using Lummo.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;
using System.Text.Json;

namespace Lummo.Server.Middlewares;

public class GlobalExceptionHandlerMiddleware(
    ILogger<GlobalExceptionHandlerMiddleware> logger) : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorResponse = exception switch
        {
            AuthenticationException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ErrorCode = "AUTHENTICATION_FAILED",
                Message = ex.Message
            },

            SecurityTokenException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ErrorCode = "INVALID_TOKEN",
                Message = ex.Message
            },

            ValidationException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = "VALIDATION_FAILED",
                Message = "Validation failed.",
                Errors = ex.Errors.Select(e => e.ErrorMessage)
            },

            ArgumentException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = "INVALID_ARGUMENT",
                Message = ex.Message
            },

            InvalidOperationException ex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status409Conflict,
                ErrorCode = "INVALID_OPERATION",
                Message = string.IsNullOrEmpty(ex.Message) ? "The requested operation is not valid." : ex.Message
            },

            NotSupportedException => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = "NOT_SUPPORTED",
                Message = "The requested operation is not supported."
            },

            _ => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorCode = "INTERNAL_ERROR",
                Message = "An unexpected error occurred. Please try again later."
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.StatusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }
}

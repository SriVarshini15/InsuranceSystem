using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.Exceptions;

namespace VehicleInsuranceSystem.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred while processing the request");

        var errorResponse = exception switch
        {
            ValidationException => new ErrorResponseDto(
                (int)HttpStatusCode.BadRequest,
                "Validation Error",
                exception.Message),

            BadHttpRequestException => new ErrorResponseDto(
                (int)HttpStatusCode.BadRequest,
                exception.GetType().Name,
                exception.Message),

            BadRequestException => new ErrorResponseDto(
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                exception.Message),

            ArgumentException => new ErrorResponseDto(
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                exception.Message),

            KeyNotFoundException => new ErrorResponseDto(
                (int)HttpStatusCode.NotFound,
                exception.GetType().Name,
                exception.Message),

            ResourceNotFoundException => new ErrorResponseDto(
                (int)HttpStatusCode.NotFound,
                "Not Found",
                exception.Message),

            DuplicateResourceException => new ErrorResponseDto(
                (int)HttpStatusCode.Conflict,
                "Duplicate Resource",
                exception.Message),

            InvalidCredentialsException => new ErrorResponseDto(
                (int)HttpStatusCode.Unauthorized,
                "Invalid Credentials",
                exception.Message),

            UnauthorizedAccessException => new ErrorResponseDto(
                (int)HttpStatusCode.Unauthorized,
                exception.GetType().Name,
                "You are not authorized to perform this action."),

            SecurityTokenException => new ErrorResponseDto(
                (int)HttpStatusCode.Unauthorized,
                "Invalid Token",
                "Your login session is invalid or expired. Please log in again."),

            ForbiddenException => new ErrorResponseDto(
                (int)HttpStatusCode.Forbidden,
                "Forbidden",
                exception.Message),

            InvalidOperationException => new ErrorResponseDto(
                (int)HttpStatusCode.BadRequest,
                exception.GetType().Name,
                exception.Message),

            BusinessRuleException => new ErrorResponseDto(
                (int)HttpStatusCode.UnprocessableEntity,
                "Business Rule Failed",
                exception.Message),

            DbUpdateConcurrencyException => new ErrorResponseDto(
                (int)HttpStatusCode.Conflict,
                "Concurrency Conflict",
                "This record was changed by another request. Please refresh and try again."),

            DbUpdateException => new ErrorResponseDto(
                (int)HttpStatusCode.Conflict,
                "Database Error",
                "The request could not be saved because it conflicts with existing data."),

            _ => new ErrorResponseDto(
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "Something went wrong while processing your request.")
        };

        httpContext.Response.StatusCode = errorResponse.StatusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}

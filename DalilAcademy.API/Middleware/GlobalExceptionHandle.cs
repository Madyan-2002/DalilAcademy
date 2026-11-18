using DalilAcademy.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Middleware
{
    public class GlobalExceptionHandler(
        IProblemDetailsService problemDetails,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            var (status, title) = exception switch
            {
                FluentValidation.ValidationException => (400, "Validation failed"),
                BadRequestException => (400, "Bad request"),
                PaymentFailedException => (400, "Payment failed"),
                UnauthorizedAccessException => (401, "Unauthorized"),
                NotFoundException => (404, "Resource not found"),
                ConflictException => (409, "Conflict"),
                HttpRequestException => (503, "Payment service unavailable"),
                _ => (500, "An unexpected error occurred")
            };

            if (status is 500 or 503)
                logger.LogError(exception, "Unhandled exception");

            context.Response.StatusCode = status;

            var detail = exception switch
            {
                NotFoundException or ConflictException or UnauthorizedAccessException
                    or BadRequestException or PaymentFailedException => exception.Message,
                HttpRequestException => "The payment provider is unreachable. Please try again later.",
                _ => null
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            };

            if (exception is FluentValidation.ValidationException ve)
                problem.Extensions["errors"] = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return await problemDetails.TryWriteAsync(
                new() { HttpContext = context, ProblemDetails = problem });
        }
    }
}
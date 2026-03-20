using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace generic_repo_uow_pattern_api.Exceptions
{
    public class TimeoutExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<TimeoutExceptionHandler> _logger;

        public TimeoutExceptionHandler(ILogger<TimeoutExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not TimeoutException)
                return false;

            _logger.LogError(exception, "Timeout: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status408RequestTimeout,
                Title = "Request Timeout",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
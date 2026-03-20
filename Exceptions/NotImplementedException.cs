using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace generic_repo_uow_pattern_api.Exceptions
{
    public class NotImplementedExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<NotImplementedExceptionHandler> _logger;

        public NotImplementedExceptionHandler(ILogger<NotImplementedExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not NotImplementedException)
                return false; // pass to next handler

            _logger.LogError(exception, "Not implemented: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status501NotImplemented,
                Title = "Not Implemented",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
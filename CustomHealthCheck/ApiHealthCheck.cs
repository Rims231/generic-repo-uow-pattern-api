using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace generic_repo_uow_pattern_api.CustomHealthCheck
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public ApiHealthCheck(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
     public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
{
    try
    {
        var response = await _httpClient.GetAsync("https://localhost:7024/health", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return new HealthCheckResult(
                status: HealthStatus.Healthy,
                description: "The API is healthy.");
        }

        return new HealthCheckResult(
            status: HealthStatus.Degraded,
            description: $"The API responded with: {(int)response.StatusCode} {response.StatusCode}.");
    }
    catch (Exception ex)
    {
        return new HealthCheckResult(
            status: HealthStatus.Unhealthy,
            description: "The API is unreachable.",
            exception: ex);
    }
}

    
    }
}

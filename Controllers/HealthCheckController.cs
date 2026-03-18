using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace generic_repo_uow_pattern_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            HealthReport healthReport = await _healthCheckService.CheckHealthAsync();
            return Ok(healthReport.Status.ToString());

            //if (healthReport.Status == HealthStatus.Healthy)
            //{
            //    return Ok("The application is healthy.");
            //}
            //else
            //{
            //    return StatusCode(StatusCodes.Status503ServiceUnavailable, "The application is unhealthy.");
            //}
        }
    }
}
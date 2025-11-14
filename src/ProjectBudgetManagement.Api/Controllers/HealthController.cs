using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectBudgetManagement.Api.Controllers;

/// <summary>
/// Provides health check endpoint for monitoring and availability verification.
/// </summary>
/// <remarks>
/// Use this endpoint to verify that the API is running and responsive.
/// Useful for load balancers, monitoring systems, and health checks.
/// 
/// **Performance**: Responds in &lt;10ms
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Checks if the API is running and responsive
    /// </summary>
    /// <returns>Health status with timestamp and version information</returns>
    /// <remarks>
    /// Returns a simple health status response indicating the API is operational.
    /// 
    /// **Response includes**:
    /// - Status: "healthy" if API is running
    /// - Timestamp: Current UTC time
    /// - Service name and version
    /// 
    /// **Performance**: Responds in &lt;10ms
    /// 
    /// Sample request:
    /// 
    ///     GET /api/health
    /// 
    /// Sample response:
    /// 
    ///     {
    ///       "status": "healthy",
    ///       "timestamp": "2025-11-14T10:30:00Z",
    ///       "service": "Project Budget Management API",
    ///       "version": "1.0.0"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">API is healthy and responsive.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Health check",
        Description = "Verifies API availability and responsiveness",
        OperationId = "HealthCheck",
        Tags = new[] { "Health" }
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "Project Budget Management API",
            version = "1.0.0"
        });
    }
}

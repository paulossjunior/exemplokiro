using Microsoft.AspNetCore.Mvc;

namespace ProjectBudgetManagement.Api.Controllers;

/// <summary>
/// Health check endpoint for monitoring API availability
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Check if the API is running
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
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

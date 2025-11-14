using Microsoft.AspNetCore.Mvc;
using ProjectBudgetManagement.Api.Services;

namespace ProjectBudgetManagement.Api.Controllers;

/// <summary>
/// Controller for exposing performance metrics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MetricsController : ControllerBase
{
    private readonly PerformanceMetricsService _metricsService;

    /// <summary>
    /// Initializes a new instance of the MetricsController class.
    /// </summary>
    /// <param name="metricsService">The performance metrics service.</param>
    public MetricsController(PerformanceMetricsService metricsService)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
    }

    /// <summary>
    /// Gets performance metrics for all endpoints.
    /// </summary>
    /// <returns>Dictionary of endpoint metrics.</returns>
    /// <response code="200">Returns the performance metrics for all endpoints.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Dictionary<string, EndpointMetricsSummary>), StatusCodes.Status200OK)]
    public ActionResult<Dictionary<string, EndpointMetricsSummary>> GetAllMetrics()
    {
        var metrics = _metricsService.GetAllMetrics();
        return Ok(metrics);
    }

    /// <summary>
    /// Gets performance metrics for a specific endpoint.
    /// </summary>
    /// <param name="method">The HTTP method (e.g., GET, POST).</param>
    /// <param name="path">The endpoint path (e.g., /api/projects).</param>
    /// <returns>Endpoint metrics summary.</returns>
    /// <response code="200">Returns the performance metrics for the specified endpoint.</response>
    /// <response code="404">If no metrics found for the specified endpoint.</response>
    [HttpGet("{method}/{*path}")]
    [ProducesResponseType(typeof(EndpointMetricsSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EndpointMetricsSummary> GetEndpointMetrics(string method, string path)
    {
        var endpoint = $"{method.ToUpper()} /{path}";
        var metrics = _metricsService.GetMetrics(endpoint);

        if (metrics == null)
        {
            return NotFound(new { message = $"No metrics found for endpoint: {endpoint}" });
        }

        return Ok(metrics);
    }

    /// <summary>
    /// Resets all collected performance metrics.
    /// </summary>
    /// <returns>Success message.</returns>
    /// <response code="200">Metrics successfully reset.</response>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ResetMetrics()
    {
        _metricsService.ResetMetrics();
        return Ok(new { message = "Performance metrics have been reset." });
    }
}

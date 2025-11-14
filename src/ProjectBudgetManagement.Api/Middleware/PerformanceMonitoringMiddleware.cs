using System.Diagnostics;
using ProjectBudgetManagement.Api.Services;

namespace ProjectBudgetManagement.Api.Middleware;

/// <summary>
/// Middleware for monitoring API endpoint performance and logging response times.
/// </summary>
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    private readonly PerformanceMetricsService _metricsService;
    private const int PerformanceThresholdMs = 100;

    /// <summary>
    /// Initializes a new instance of the PerformanceMonitoringMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="metricsService">The performance metrics service.</param>
    public PerformanceMonitoringMiddleware(
        RequestDelegate next, 
        ILogger<PerformanceMonitoringMiddleware> logger,
        PerformanceMetricsService metricsService)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
    }

    /// <summary>
    /// Invokes the middleware to monitor request performance.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;

            // Record metrics
            _metricsService.RecordRequest(requestPath, requestMethod, elapsedMs, statusCode);

            // Log all requests with their response times
            _logger.LogInformation(
                "Request {Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}",
                requestMethod,
                requestPath,
                elapsedMs,
                statusCode);

            // Log warning if request exceeds performance threshold
            if (elapsedMs > PerformanceThresholdMs)
            {
                _logger.LogWarning(
                    "PERFORMANCE ALERT: Request {Method} {Path} took {ElapsedMs}ms (threshold: {ThresholdMs}ms) - Status: {StatusCode}",
                    requestMethod,
                    requestPath,
                    elapsedMs,
                    PerformanceThresholdMs,
                    statusCode);
            }

            // Add response time header for monitoring (only if response hasn't started)
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Response-Time-Ms"] = elapsedMs.ToString();
            }
        }
    }
}

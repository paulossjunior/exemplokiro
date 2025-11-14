using System.Collections.Concurrent;

namespace ProjectBudgetManagement.Api.Services;

/// <summary>
/// Service for collecting and exposing performance metrics.
/// </summary>
public class PerformanceMetricsService
{
    private readonly ConcurrentDictionary<string, EndpointMetrics> _endpointMetrics = new();
    private const int MaxSampleSize = 1000;

    /// <summary>
    /// Records a request completion with its response time.
    /// </summary>
    /// <param name="endpoint">The endpoint path.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="responseTimeMs">The response time in milliseconds.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public void RecordRequest(string endpoint, string method, long responseTimeMs, int statusCode)
    {
        var key = $"{method} {endpoint}";
        var metrics = _endpointMetrics.GetOrAdd(key, _ => new EndpointMetrics(key));
        
        metrics.RecordRequest(responseTimeMs, statusCode);
    }

    /// <summary>
    /// Gets metrics for all endpoints.
    /// </summary>
    /// <returns>Dictionary of endpoint metrics.</returns>
    public Dictionary<string, EndpointMetricsSummary> GetAllMetrics()
    {
        return _endpointMetrics.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.GetSummary());
    }

    /// <summary>
    /// Gets metrics for a specific endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint key (e.g., "GET /api/projects").</param>
    /// <returns>Endpoint metrics summary if found.</returns>
    public EndpointMetricsSummary? GetMetrics(string endpoint)
    {
        return _endpointMetrics.TryGetValue(endpoint, out var metrics) 
            ? metrics.GetSummary() 
            : null;
    }

    /// <summary>
    /// Resets all collected metrics.
    /// </summary>
    public void ResetMetrics()
    {
        _endpointMetrics.Clear();
    }
}

/// <summary>
/// Metrics for a specific endpoint.
/// </summary>
public class EndpointMetrics
{
    private readonly string _endpoint;
    private readonly ConcurrentQueue<long> _responseTimes = new();
    private long _totalRequests;
    private long _totalResponseTime;
    private long _requestsExceedingThreshold;
    private readonly object _lock = new();

    public EndpointMetrics(string endpoint)
    {
        _endpoint = endpoint;
    }

    public void RecordRequest(long responseTimeMs, int statusCode)
    {
        lock (_lock)
        {
            _totalRequests++;
            _totalResponseTime += responseTimeMs;

            if (responseTimeMs > 100)
            {
                _requestsExceedingThreshold++;
            }

            _responseTimes.Enqueue(responseTimeMs);

            // Keep only recent samples
            while (_responseTimes.Count > 1000)
            {
                _responseTimes.TryDequeue(out _);
            }
        }
    }

    public EndpointMetricsSummary GetSummary()
    {
        lock (_lock)
        {
            var times = _responseTimes.ToArray();
            
            return new EndpointMetricsSummary
            {
                Endpoint = _endpoint,
                TotalRequests = _totalRequests,
                AverageResponseTimeMs = _totalRequests > 0 ? (double)_totalResponseTime / _totalRequests : 0,
                MinResponseTimeMs = times.Length > 0 ? times.Min() : 0,
                MaxResponseTimeMs = times.Length > 0 ? times.Max() : 0,
                MedianResponseTimeMs = CalculateMedian(times),
                P95ResponseTimeMs = CalculatePercentile(times, 0.95),
                P99ResponseTimeMs = CalculatePercentile(times, 0.99),
                RequestsExceedingThreshold = _requestsExceedingThreshold,
                PercentageExceedingThreshold = _totalRequests > 0 
                    ? (double)_requestsExceedingThreshold / _totalRequests * 100 
                    : 0
            };
        }
    }

    private static double CalculateMedian(long[] values)
    {
        if (values.Length == 0) return 0;
        
        var sorted = values.OrderBy(x => x).ToArray();
        var mid = sorted.Length / 2;
        
        return sorted.Length % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }

    private static double CalculatePercentile(long[] values, double percentile)
    {
        if (values.Length == 0) return 0;
        
        var sorted = values.OrderBy(x => x).ToArray();
        var index = (int)Math.Ceiling(percentile * sorted.Length) - 1;
        index = Math.Max(0, Math.Min(sorted.Length - 1, index));
        
        return sorted[index];
    }
}

/// <summary>
/// Summary of endpoint performance metrics.
/// </summary>
public class EndpointMetricsSummary
{
    /// <summary>
    /// Gets or sets the endpoint identifier.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of requests.
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// Gets or sets the average response time in milliseconds.
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the minimum response time in milliseconds.
    /// </summary>
    public long MinResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the maximum response time in milliseconds.
    /// </summary>
    public long MaxResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the median response time in milliseconds.
    /// </summary>
    public double MedianResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the 95th percentile response time in milliseconds.
    /// </summary>
    public double P95ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the 99th percentile response time in milliseconds.
    /// </summary>
    public double P99ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the number of requests exceeding the 100ms threshold.
    /// </summary>
    public long RequestsExceedingThreshold { get; set; }

    /// <summary>
    /// Gets or sets the percentage of requests exceeding the threshold.
    /// </summary>
    public double PercentageExceedingThreshold { get; set; }
}

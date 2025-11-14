namespace ProjectBudgetManagement.IntegrationTests.Infrastructure;

/// <summary>
/// Configuration settings for integration tests.
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Maximum allowed response time for API endpoints in milliseconds.
    /// </summary>
    public const int MaxResponseTimeMs = 100;

    /// <summary>
    /// Maximum allowed response time for integration tests (more lenient) in milliseconds.
    /// </summary>
    public const int MaxIntegrationTestResponseTimeMs = 500;

    /// <summary>
    /// Test user ID used for authentication in tests.
    /// </summary>
    public static readonly Guid TestUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Test authorization token.
    /// </summary>
    public const string TestAuthToken = "Bearer test-token";

    /// <summary>
    /// Default test database password.
    /// </summary>
    public const string TestDatabasePassword = "Test@1234";

    /// <summary>
    /// Default timeout for database operations in seconds.
    /// </summary>
    public const int DatabaseOperationTimeoutSeconds = 30;
}

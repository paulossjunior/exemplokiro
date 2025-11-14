using ProjectBudgetManagement.Application.Services;

namespace ProjectBudgetManagement.Application.Commands;

/// <summary>
/// Handler for generating accountability reports.
/// </summary>
public class GenerateAccountabilityReportCommandHandler
{
    private readonly ReportingService _reportingService;

    /// <summary>
    /// Initializes a new instance of the GenerateAccountabilityReportCommandHandler class.
    /// </summary>
    /// <param name="reportingService">The reporting service.</param>
    public GenerateAccountabilityReportCommandHandler(ReportingService reportingService)
    {
        _reportingService = reportingService ?? throw new ArgumentNullException(nameof(reportingService));
    }

    /// <summary>
    /// Handles the generate accountability report command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated accountability report.</returns>
    public async Task<AccountabilityReport> HandleAsync(
        GenerateAccountabilityReportCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return await _reportingService.GenerateAccountabilityReportAsync(
            command.ProjectId,
            cancellationToken);
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// DTO for dashboard budget metrics
/// </summary>
public class DashboardMetricsDto
{
    /// <summary>
    /// Total amount consumed from the budget
    /// </summary>
    [Required]
    public decimal Consumed { get; set; }

    /// <summary>
    /// Remaining budget amount
    /// </summary>
    [Required]
    public decimal Remaining { get; set; }

    /// <summary>
    /// Yield or interest earned on the account
    /// </summary>
    [Required]
    public decimal Yield { get; set; }

    /// <summary>
    /// Total budget amount
    /// </summary>
    [Required]
    public decimal Total { get; set; }

    /// <summary>
    /// Project identifier
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ProjectName { get; set; } = string.Empty;
}

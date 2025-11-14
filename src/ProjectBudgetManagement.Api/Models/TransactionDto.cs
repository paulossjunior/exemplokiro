using System.ComponentModel.DataAnnotations;

namespace ProjectBudgetManagement.Api.Models;

/// <summary>
/// DTO for transaction data
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Transaction identifier
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Payment method (e.g., Pix, Boleto, Credit Card)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Transaction amount
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Transaction date and time
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Company CNPJ (Brazilian tax ID)
    /// </summary>
    [Required]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Company name (optional)
    /// </summary>
    [StringLength(200)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Transaction status
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Expense category
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Associated project identifier
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
}

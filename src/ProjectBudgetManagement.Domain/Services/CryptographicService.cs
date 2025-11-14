using System.Security.Cryptography;
using System.Text;

namespace ProjectBudgetManagement.Domain.Services;

/// <summary>
/// Service for cryptographic operations including SHA-256 hashing.
/// </summary>
public class CryptographicService
{
    /// <summary>
    /// Computes SHA-256 hash of the input data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>The hexadecimal string representation of the hash.</returns>
    public string ComputeHash(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));
        }

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Verifies that the provided hash matches the computed hash of the data.
    /// </summary>
    /// <param name="data">The original data.</param>
    /// <param name="expectedHash">The expected hash value.</param>
    /// <returns>True if hashes match, false otherwise.</returns>
    public bool VerifyHash(string data, string expectedHash)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));
        }

        if (string.IsNullOrEmpty(expectedHash))
        {
            throw new ArgumentException("Expected hash cannot be null or empty.", nameof(expectedHash));
        }

        var computedHash = ComputeHash(data);
        return string.Equals(computedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Computes hash for transaction data.
    /// </summary>
    /// <param name="amount">Transaction amount.</param>
    /// <param name="date">Transaction date.</param>
    /// <param name="classification">Transaction classification.</param>
    /// <param name="bankAccountId">Bank account ID.</param>
    /// <param name="accountingAccountId">Accounting account ID.</param>
    /// <param name="createdBy">User ID who created the transaction.</param>
    /// <returns>The computed hash.</returns>
    public string ComputeTransactionHash(
        decimal amount,
        DateTime date,
        int classification,
        Guid bankAccountId,
        Guid accountingAccountId,
        Guid createdBy)
    {
        var data = $"{amount}|{date:O}|{classification}|{bankAccountId}|{accountingAccountId}|{createdBy}";
        return ComputeHash(data);
    }

    /// <summary>
    /// Computes hash for audit entry data.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="actionType">Action type.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="entityId">Entity ID.</param>
    /// <param name="timestamp">Timestamp.</param>
    /// <param name="previousValue">Previous value (can be null).</param>
    /// <param name="newValue">New value (can be null).</param>
    /// <returns>The computed hash.</returns>
    public string ComputeAuditEntryHash(
        Guid userId,
        string actionType,
        string entityType,
        Guid entityId,
        DateTime timestamp,
        string? previousValue,
        string? newValue)
    {
        var data = $"{userId}|{actionType}|{entityType}|{entityId}|{timestamp:O}|{previousValue ?? ""}|{newValue ?? ""}";
        return ComputeHash(data);
    }
}

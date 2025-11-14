using System.Security.Cryptography;
using System.Text;

namespace ProjectBudgetManagement.Domain.Services;

/// <summary>
/// Service for creating and validating digital signatures for non-repudiation.
/// </summary>
public class DigitalSignatureService
{
    private readonly string _secretKey;

    /// <summary>
    /// Initializes a new instance of the DigitalSignatureService class.
    /// </summary>
    /// <param name="secretKey">The secret key for HMAC signature generation.</param>
    public DigitalSignatureService(string secretKey)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("Secret key cannot be null or empty.", nameof(secretKey));
        }

        _secretKey = secretKey;
    }

    /// <summary>
    /// Generates a digital signature for the provided data using HMAC-SHA256.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="userId">The user ID creating the signature.</param>
    /// <returns>The digital signature as a hexadecimal string.</returns>
    public string GenerateSignature(string data, Guid userId)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));
        }

        var dataWithUser = $"{userId}|{data}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var bytes = Encoding.UTF8.GetBytes(dataWithUser);
        var hashBytes = hmac.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Validates a digital signature against the provided data.
    /// </summary>
    /// <param name="data">The original data.</param>
    /// <param name="userId">The user ID who created the signature.</param>
    /// <param name="signature">The signature to validate.</param>
    /// <returns>True if signature is valid, false otherwise.</returns>
    public bool ValidateSignature(string data, Guid userId, string signature)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));
        }

        if (string.IsNullOrEmpty(signature))
        {
            throw new ArgumentException("Signature cannot be null or empty.", nameof(signature));
        }

        var expectedSignature = GenerateSignature(data, userId);
        return string.Equals(expectedSignature, signature, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generates a signature for transaction data.
    /// </summary>
    /// <param name="amount">Transaction amount.</param>
    /// <param name="date">Transaction date.</param>
    /// <param name="classification">Transaction classification.</param>
    /// <param name="bankAccountId">Bank account ID.</param>
    /// <param name="accountingAccountId">Accounting account ID.</param>
    /// <param name="userId">User ID creating the transaction.</param>
    /// <returns>The digital signature.</returns>
    public string GenerateTransactionSignature(
        decimal amount,
        DateTime date,
        int classification,
        Guid bankAccountId,
        Guid accountingAccountId,
        Guid userId)
    {
        var data = $"{amount}|{date:O}|{classification}|{bankAccountId}|{accountingAccountId}";
        return GenerateSignature(data, userId);
    }

    /// <summary>
    /// Generates a signature for audit entry data.
    /// </summary>
    /// <param name="actionType">Action type.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="entityId">Entity ID.</param>
    /// <param name="timestamp">Timestamp.</param>
    /// <param name="userId">User ID performing the action.</param>
    /// <returns>The digital signature.</returns>
    public string GenerateAuditSignature(
        string actionType,
        string entityType,
        Guid entityId,
        DateTime timestamp,
        Guid userId)
    {
        var data = $"{actionType}|{entityType}|{entityId}|{timestamp:O}";
        return GenerateSignature(data, userId);
    }
}

using System.Text.RegularExpressions;

namespace ProjectBudgetManagement.Domain.ValueObjects;

/// <summary>
/// Represents an accounting account identifier with pattern validation.
/// </summary>
public class AccountIdentifier : IEquatable<AccountIdentifier>
{
    private static readonly Regex IdentifierPattern = new Regex(
        @"^\d{4}\.\d{2}\.\d{4}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the identifier value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the AccountIdentifier class.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when the identifier format is invalid.</exception>
    public AccountIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Account identifier cannot be null or empty.", nameof(value));
        }

        if (!IdentifierPattern.IsMatch(value))
        {
            throw new ArgumentException(
                "Account identifier must follow the pattern XXXX.XX.XXXX where X is a digit.",
                nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Determines whether the specified AccountIdentifier is equal to the current AccountIdentifier.
    /// </summary>
    public bool Equals(AccountIdentifier? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current AccountIdentifier.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as AccountIdentifier);
    }

    /// <summary>
    /// Returns the hash code for this AccountIdentifier.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the AccountIdentifier.
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(AccountIdentifier? left, AccountIdentifier? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(AccountIdentifier? left, AccountIdentifier? right)
    {
        return !(left == right);
    }
}

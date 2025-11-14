namespace ProjectBudgetManagement.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with validation.
/// </summary>
public class Money : IEquatable<Money>
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency code (e.g., "USD", "EUR").
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Initializes a new instance of the Money class.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The currency code.</param>
    /// <exception cref="ArgumentException">Thrown when amount is not positive or currency is invalid.</exception>
    public Money(decimal amount, string currency = "USD")
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Determines whether the specified Money is equal to the current Money.
    /// </summary>
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Money.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Money);
    }

    /// <summary>
    /// Returns the hash code for this Money.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    /// <summary>
    /// Returns a string representation of the Money.
    /// </summary>
    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }
}

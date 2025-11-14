namespace ProjectBudgetManagement.Domain.Entities;

/// <summary>
/// Represents a person in the system who can act as a project coordinator.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets or sets the unique identifier for the person.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the person's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's identification number (must be unique).
    /// </summary>
    public string IdentificationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the person was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of projects this person coordinates.
    /// </summary>
    public virtual ICollection<Project> CoordinatedProjects { get; set; } = new List<Project>();

    /// <summary>
    /// Validates the person entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Person name is required.");
        }

        if (string.IsNullOrWhiteSpace(IdentificationNumber))
        {
            throw new InvalidOperationException("Person identification number is required.");
        }
    }
}

---
inclusion: manual
---

# Back-End Code Quality Standards

## SOLID Principles

- **Single Responsibility**: Each class/module should have one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Subtypes must be substitutable for their base types
- **Interface Segregation**: Clients shouldn't depend on interfaces they don't use
- **Dependency Inversion**: Depend on abstractions, not concretions

## Design Patterns

Use appropriate design patterns when they solve real problems:

- **Creational**: Factory, Builder, Singleton (use sparingly)
- **Structural**: Adapter, Decorator, Facade, Proxy
- **Behavioral**: Strategy, Observer, Command, Template Method

Avoid patterns for the sake of patterns - use them when they add clarity.

## Clean Code Practices

- Use meaningful, descriptive names for variables, functions, and classes
- Keep functions small and focused on a single task
- Avoid deep nesting - prefer early returns
- Write self-documenting code; comments explain "why", not "what"
- Follow DRY (Don't Repeat Yourself) principle
- Keep code simple - avoid premature optimization

## Anti-Patterns to Avoid

- **God Objects**: Classes that know or do too much
- **Spaghetti Code**: Unstructured, tangled control flow
- **Magic Numbers**: Use named constants instead
- **Tight Coupling**: Minimize dependencies between modules
- **Premature Optimization**: Focus on clarity first
- **Copy-Paste Programming**: Extract shared logic into reusable functions
- **Long Parameter Lists**: Use objects or builders for complex parameters

## Language Requirements

- **All code, comments, documentation, and commit messages must be in English**
- Variable names, function names, class names: English only
- Code comments and documentation: English only
- Git commit messages: English only
- README and technical documentation: English only

## Documentation Standards

Document all classes and functions using C# XML documentation comments:

```csharp
/// <summary>
/// Calculates the balance for a bank account.
/// </summary>
/// <param name="transactions">The list of transactions.</param>
/// <returns>The calculated balance.</returns>
/// <exception cref="ArgumentNullException">Thrown when transactions is null.</exception>
public decimal CalculateBalance(IEnumerable<Transaction> transactions)
{
    // Implementation
}
```

### What to Document

- **Classes**: Purpose, responsibilities, and usage examples
- **Methods**: Parameters, return values, exceptions, and side effects
- **Public APIs**: Comprehensive documentation for external consumers
- **Complex Logic**: Explain the "why" behind non-obvious implementations

### Documentation Quality

- Keep documentation up-to-date with code changes
- Use clear, concise language
- Include examples for complex functionality
- Document edge cases and limitations

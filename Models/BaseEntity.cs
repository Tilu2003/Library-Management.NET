namespace LibraryManagementSystem.Models;

/// <summary>
/// Abstract base class applying OOP Abstraction + Inheritance.
/// All entities share a common ID and audit fields.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Polymorphism: every entity can describe itself
    public abstract string GetDisplayName();
}

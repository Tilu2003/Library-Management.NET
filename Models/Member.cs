using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

/// <summary>
/// Member entity — inherits BaseEntity (OOP Inheritance).
/// Demonstrates Encapsulation with private fields and validation.
/// </summary>
public class Member : BaseEntity
{
    private string _name = string.Empty;
    private string _contactInfo = string.Empty;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

    [Required, MaxLength(300)]
    public string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }

    [Required, MaxLength(200)]
    public string ContactInfo
    {
        get => _contactInfo;
        set => _contactInfo = value?.Trim() ?? string.Empty;
    }

    public DateTime MembershipDate { get; set; } = DateTime.Today;

    // Navigation: One member → Many transactions
    public virtual ICollection<BorrowTransaction> Transactions { get; set; } = new List<BorrowTransaction>();

    // Polymorphism: override from BaseEntity
    public override string GetDisplayName() => $"{Name} ({ContactInfo})";
}

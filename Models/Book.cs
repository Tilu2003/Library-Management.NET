using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;


/// Book entity — inherits BaseEntity 
public class Book : BaseEntity
{
    // ── Encapsulation: private fields, controlled via properties 
    private string _title = string.Empty;
    private string _author = string.Empty;
    private string _isbn = string.Empty;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

    [Required, MaxLength(500)]
    public string Title
    {
        get => _title;
        set => _title = value?.Trim() ?? string.Empty;
    }

    [Required, MaxLength(300)]
    public string Author
    {
        get => _author;
        set => _author = value?.Trim() ?? string.Empty;
    }

    [Required, MaxLength(20)]
    public string ISBN
    {
        get => _isbn;
        set => _isbn = value?.Trim() ?? string.Empty;
    }

    [MaxLength(100)]
    public string Genre { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    // (One-to-Many: a book can have many transactions)
    public virtual ICollection<BorrowTransaction> Transactions { get; set; } = new List<BorrowTransaction>();

    // Polymorphism: override abstract method from BaseEntity
    public override string GetDisplayName() => $"{Title} by {Author} (ISBN: {ISBN})";
}

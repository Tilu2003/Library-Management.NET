using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;


/// Represents a single borrowing event.
/// (One-to-One per transaction and   One-to-Many ).

public class BorrowTransaction : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

   
    [Required]
    public int BookId { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public DateTime BorrowDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);

    public DateTime? ReturnDate { get; set; }   // null = not yet returned

    [ForeignKey(nameof(BookId))]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey(nameof(MemberId))]
    public virtual Member Member { get; set; } = null!;

    
    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue  => !IsReturned && DateTime.Today > DueDate;

    public override string GetDisplayName() =>
        $"Transaction #{Id} — {Book?.Title ?? "Unknown"} → {Member?.Name ?? "Unknown"}";
}

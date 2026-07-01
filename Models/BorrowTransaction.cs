using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

/// <summary>
/// Represents a single borrowing event.
/// Links Book ↔ Member (One-to-One per transaction; One-to-Many overall).
/// </summary>
public class BorrowTransaction : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

    // ── Foreign Keys ──────────────────────────────────────────────────────────
    [Required]
    public int BookId { get; set; }

    [Required]
    public int MemberId { get; set; }

    // ── Dates ─────────────────────────────────────────────────────────────────
    [Required]
    public DateTime BorrowDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);

    public DateTime? ReturnDate { get; set; }   // null = not yet returned

    // ── Navigation Properties ─────────────────────────────────────────────────
    [ForeignKey(nameof(BookId))]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey(nameof(MemberId))]
    public virtual Member Member { get; set; } = null!;

    // ── Computed helpers ──────────────────────────────────────────────────────
    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue  => !IsReturned && DateTime.Today > DueDate;

    public override string GetDisplayName() =>
        $"Transaction #{Id} — {Book?.Title ?? "Unknown"} → {Member?.Name ?? "Unknown"}";
}

using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<BorrowTransaction> BorrowTransactions => Set<BorrowTransaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=LibraryManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => b.ISBN).IsUnique();
            entity.Property(b => b.Title).IsRequired().HasMaxLength(500);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(300);
            entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            entity.Property(b => b.Genre).HasMaxLength(100);
            entity.Property(b => b.IsAvailable).HasDefaultValue(true);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("Members");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(300);
            entity.Property(m => m.ContactInfo).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<BorrowTransaction>(entity =>
        {
            entity.ToTable("BorrowTransactions");
            entity.HasKey(t => t.Id);

            entity.HasOne(t => t.Member)
                  .WithMany(m => m.Transactions)
                  .HasForeignKey(t => t.MemberId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Book)
                  .WithMany(b => b.Transactions)
                  .HasForeignKey(t => t.BookId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", ISBN = "978-0132350884", Genre = "Programming", IsAvailable = true },
            new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "David Thomas", ISBN = "978-0135957059", Genre = "Programming", IsAvailable = true },
            new Book { Id = 3, Title = "Design Patterns", Author = "Gang of Four", ISBN = "978-0201633610", Genre = "Programming", IsAvailable = true }
        );

        modelBuilder.Entity<Member>().HasData(
            new Member { Id = 1, Name = "Alice Johnson", ContactInfo = "alice@email.com", MembershipDate = new DateTime(2024, 1, 1) },
            new Member { Id = 2, Name = "Bob Smith", ContactInfo = "bob@email.com", MembershipDate = new DateTime(2024, 3, 15) }
        );
    }
}
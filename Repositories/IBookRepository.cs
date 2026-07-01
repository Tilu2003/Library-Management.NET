using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories;

/// <summary>
/// Extends generic IRepository with book-specific queries.
/// Demonstrates Interface Segregation (SOLID-I).
/// </summary>
public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<Book?> GetByIsbnAsync(string isbn);
}

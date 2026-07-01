using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<Book?> GetByIsbnAsync(string isbn);
}

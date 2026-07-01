using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Serilog;

namespace LibraryManagementSystem.Services;

/// <summary>
/// Business logic for books.
/// Single Responsibility: only book-related logic here (SOLID-S).
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _repo;

    public BookService(IBookRepository repo) => _repo = repo;

    public Task<IEnumerable<Book>> GetAllBooksAsync()       => _repo.GetAllAsync();
    public Task<IEnumerable<Book>> GetAvailableBooksAsync() => _repo.GetAvailableBooksAsync();
    public Task<IEnumerable<Book>> SearchBooksAsync(string term) => _repo.SearchBooksAsync(term);
    public Task<Book?> GetBookByIdAsync(int id)             => _repo.GetByIdAsync(id);

    public async Task<Book> AddBookAsync(Book book)
    {
        // Business rule: ISBN must be unique
        var existing = await _repo.GetByIsbnAsync(book.ISBN);
        if (existing != null)
            throw new InvalidOperationException($"A book with ISBN '{book.ISBN}' already exists.");

        return await _repo.AddAsync(book);
    }

    public async Task<Book> UpdateBookAsync(Book book)
    {
        if (!await _repo.ExistsAsync(book.Id))
            throw new KeyNotFoundException($"Book {book.Id} not found.");

        // Check ISBN uniqueness excluding self
        var existing = await _repo.GetByIsbnAsync(book.ISBN);
        if (existing != null && existing.Id != book.Id)
            throw new InvalidOperationException($"ISBN '{book.ISBN}' is already assigned to another book.");

        return await _repo.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Book {id} not found.");

        if (!book.IsAvailable)
            throw new InvalidOperationException("Cannot delete a book that is currently borrowed.");

        await _repo.DeleteAsync(id);
        Log.Information("Book {Id} deleted", id);
    }
}

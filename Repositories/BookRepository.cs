using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LibraryManagementSystem.Repositories;

///  LINQ queries and EF Core for data access.

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        try
        {
            return await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching all books");
            throw;
        }
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Books
                .Include(b => b.Transactions)
                    .ThenInclude(t => t.Member)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching book {BookId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _context.Books
            .Where(b => b.IsAvailable)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var term = searchTerm.ToLower();

        // LINQ query 
        return await _context.Books
            .Where(b => b.Title.ToLower().Contains(term)
                     || b.Author.ToLower().Contains(term)
                     || b.ISBN.ToLower().Contains(term)
                     || b.Genre.ToLower().Contains(term))
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<Book> AddAsync(Book book)
    {
        try
        {
            book.CreatedAt = DateTime.UtcNow;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            Log.Information("Book added: {Title}", book.Title);
            return book;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding book {Title}", book.Title);
            throw;
        }
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        try
        {
            book.UpdatedAt = DateTime.UtcNow;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            Log.Information("Book updated: {Title}", book.Title);
            return book;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating book {BookId}", book.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id)
            ?? throw new KeyNotFoundException($"Book {id} not found.");
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        Log.Information("Book deleted: {BookId}", id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Books.AnyAsync(b => b.Id == id);
    }
}

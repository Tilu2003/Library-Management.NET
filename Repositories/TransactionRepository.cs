using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LibraryManagementSystem.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly LibraryDbContext _context;

    public TransactionRepository(LibraryDbContext context) => _context = context;

    private IQueryable<BorrowTransaction> WithIncludes() =>
        _context.BorrowTransactions
            .Include(t => t.Book)
            .Include(t => t.Member);

    public async Task<IEnumerable<BorrowTransaction>> GetAllAsync() =>
        await WithIncludes().OrderByDescending(t => t.BorrowDate).ToListAsync();

    public async Task<BorrowTransaction?> GetByIdAsync(int id) =>
        await WithIncludes().FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync() =>
        await WithIncludes()
            .Where(t => t.ReturnDate == null)
            .OrderBy(t => t.DueDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync() =>
        await WithIncludes()
            .Where(t => t.ReturnDate == null && t.DueDate < DateTime.Today)
            .OrderBy(t => t.DueDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowTransaction>> GetTransactionsByMemberAsync(int memberId) =>
        await WithIncludes()
            .Where(t => t.MemberId == memberId)
            .OrderByDescending(t => t.BorrowDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowTransaction>> GetTransactionsByBookAsync(int bookId) =>
        await WithIncludes()
            .Where(t => t.BookId == bookId)
            .OrderByDescending(t => t.BorrowDate)
            .ToListAsync();

    public async Task<BorrowTransaction> AddAsync(BorrowTransaction transaction)
    {
        transaction.CreatedAt = DateTime.UtcNow;
        _context.BorrowTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        Log.Information("Transaction created: Book {BookId} → Member {MemberId}", transaction.BookId, transaction.MemberId);
        return transaction;
    }

    public async Task<BorrowTransaction> UpdateAsync(BorrowTransaction transaction)
    {
        transaction.UpdatedAt = DateTime.UtcNow;
        _context.BorrowTransactions.Update(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task DeleteAsync(int id)
    {
        var t = await _context.BorrowTransactions.FindAsync(id)
            ?? throw new KeyNotFoundException($"Transaction {id} not found.");
        _context.BorrowTransactions.Remove(t);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.BorrowTransactions.AnyAsync(t => t.Id == id);
}

using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Serilog;

namespace LibraryManagementSystem.Services;

// ── MemberService ─────────────────────────────────────────────────────────────

public class MemberService : IMemberService
{
    private readonly IMemberRepository _repo;
    public MemberService(IMemberRepository repo) => _repo = repo;

    public Task<IEnumerable<Member>> GetAllMembersAsync()         => _repo.GetAllAsync();
    public Task<IEnumerable<Member>> SearchMembersAsync(string t) => _repo.SearchMembersAsync(t);
    public Task<Member?> GetMemberByIdAsync(int id)               => _repo.GetByIdAsync(id);

    public async Task<Member> AddMemberAsync(Member member)
    {
        if (string.IsNullOrWhiteSpace(member.Name))
            throw new ArgumentException("Member name is required.");
        return await _repo.AddAsync(member);
    }

    public async Task<Member> UpdateMemberAsync(Member member)
    {
        if (!await _repo.ExistsAsync(member.Id))
            throw new KeyNotFoundException($"Member {member.Id} not found.");
        return await _repo.UpdateAsync(member);
    }

    public async Task DeleteMemberAsync(int id)
    {
        var member = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Member {id} not found.");

        // Business rule: cannot delete member with active borrows
        if (member.Transactions.Any(t => t.ReturnDate == null))
            throw new InvalidOperationException("Cannot delete a member who still has borrowed books.");

        await _repo.DeleteAsync(id);
    }
}

// ── TransactionService ────────────────────────────────────────────────────────

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _txRepo;
    private readonly IBookRepository        _bookRepo;
    private readonly IMemberRepository      _memberRepo;

    public TransactionService(
        ITransactionRepository txRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo)
    {
        _txRepo     = txRepo;
        _bookRepo   = bookRepo;
        _memberRepo = memberRepo;
    }

    public Task<IEnumerable<BorrowTransaction>> GetAllTransactionsAsync()    => _txRepo.GetAllAsync();
    public Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync() => _txRepo.GetActiveTransactionsAsync();
    public Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync()=> _txRepo.GetOverdueTransactionsAsync();

    public async Task<BorrowTransaction> BorrowBookAsync(int bookId, int memberId, DateTime dueDate)
    {
        // Validate book exists and is available
        var book = await _bookRepo.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException("Book not found.");

        if (!book.IsAvailable)
            throw new InvalidOperationException($"'{book.Title}' is not available for borrowing.");

        // Validate member exists
        if (!await _memberRepo.ExistsAsync(memberId))
            throw new KeyNotFoundException("Member not found.");

        if (dueDate <= DateTime.Today)
            throw new ArgumentException("Due date must be in the future.");

        // Create transaction
        var tx = new BorrowTransaction
        {
            BookId     = bookId,
            MemberId   = memberId,
            BorrowDate = DateTime.Today,
            DueDate    = dueDate
        };

        await _txRepo.AddAsync(tx);

        // Mark book unavailable
        book.IsAvailable = false;
        await _bookRepo.UpdateAsync(book);

        Log.Information("Book borrowed: {BookId} by Member {MemberId}", bookId, memberId);
        return tx;
    }

    public async Task<BorrowTransaction> ReturnBookAsync(int transactionId)
    {
        var tx = await _txRepo.GetByIdAsync(transactionId)
            ?? throw new KeyNotFoundException("Transaction not found.");

        if (tx.IsReturned)
            throw new InvalidOperationException("This book has already been returned.");

        tx.ReturnDate = DateTime.Today;
        await _txRepo.UpdateAsync(tx);

        // Mark book available again
        var book = await _bookRepo.GetByIdAsync(tx.BookId)
            ?? throw new KeyNotFoundException("Associated book not found.");
        book.IsAvailable = true;
        await _bookRepo.UpdateAsync(book);

        Log.Information("Book returned: Transaction {TxId}", transactionId);
        return tx;
    }
}

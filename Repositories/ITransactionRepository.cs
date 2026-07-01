using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories;

public interface ITransactionRepository : IRepository<BorrowTransaction>
{
    Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync();
    Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync();
    Task<IEnumerable<BorrowTransaction>> GetTransactionsByMemberAsync(int memberId);
    Task<IEnumerable<BorrowTransaction>> GetTransactionsByBookAsync(int bookId);
}

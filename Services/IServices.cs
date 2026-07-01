using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services;


public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<IEnumerable<Book>> SearchBooksAsync(string term);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> AddBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
}

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<IEnumerable<Member>> SearchMembersAsync(string term);
    Task<Member?> GetMemberByIdAsync(int id);
    Task<Member> AddMemberAsync(Member member);
    Task<Member> UpdateMemberAsync(Member member);
    Task DeleteMemberAsync(int id);
}

public interface ITransactionService
{
    Task<IEnumerable<BorrowTransaction>> GetAllTransactionsAsync();
    Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync();
    Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync();
    Task<BorrowTransaction> BorrowBookAsync(int bookId, int memberId, DateTime dueDate);
    Task<BorrowTransaction> ReturnBookAsync(int transactionId);
}

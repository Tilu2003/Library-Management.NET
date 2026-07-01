using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm);
    Task<IEnumerable<Member>> GetMembersWithActiveBorrowsAsync();
}

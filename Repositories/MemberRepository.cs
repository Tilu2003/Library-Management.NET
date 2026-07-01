using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LibraryManagementSystem.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryDbContext _context;

    public MemberRepository(LibraryDbContext context) => _context = context;

    public async Task<IEnumerable<Member>> GetAllAsync() =>
        await _context.Members.OrderBy(m => m.Name).ToListAsync();

    public async Task<Member?> GetByIdAsync(int id) =>
        await _context.Members
            .Include(m => m.Transactions)
                .ThenInclude(t => t.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var term = searchTerm.ToLower();
        return await _context.Members
            .Where(m => m.Name.ToLower().Contains(term)
                     || m.ContactInfo.ToLower().Contains(term))
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersWithActiveBorrowsAsync() =>
        await _context.Members
            .Where(m => m.Transactions.Any(t => t.ReturnDate == null))
            .Include(m => m.Transactions)
            .ToListAsync();

    public async Task<Member> AddAsync(Member member)
    {
        member.CreatedAt = DateTime.UtcNow;
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        Log.Information("Member added: {Name}", member.Name);
        return member;
    }

    public async Task<Member> UpdateAsync(Member member)
    {
        member.UpdatedAt = DateTime.UtcNow;
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _context.Members.FindAsync(id)
            ?? throw new KeyNotFoundException($"Member {id} not found.");
        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Members.AnyAsync(m => m.Id == id);
}

using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories.Implementations;

public class LoanRepository : ILoanRepository
{
    private readonly ApplicationDbContext _context;

    public LoanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Loan>> GetAllAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Student)
            .ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Student)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Book?> GetBookByIdAsync(int bookId)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<bool> StudentExistsAsync(int studentId)
    {
        return await _context.Students
            .AnyAsync(s => s.Id == studentId);
    }

    public async Task AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);

        // Le livre est suivi par EF Core.
        // On enregistre donc aussi la diminution du stock.
        await _context.SaveChangesAsync();
    }

    public async Task SaveReturnAsync(Loan loan)
    {
        await _context.SaveChangesAsync();
    }
}
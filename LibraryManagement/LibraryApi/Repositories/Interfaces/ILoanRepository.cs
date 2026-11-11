using LibraryApi.Models;

namespace LibraryApi.Repositories.Interfaces;

public interface ILoanRepository
{
    Task<List<Loan>> GetAllAsync();

    Task<Loan?> GetByIdAsync(int id);

    Task<Book?> GetBookByIdAsync(int bookId);

    Task<bool> StudentExistsAsync(int studentId);

    Task AddAsync(Loan loan);

    Task SaveReturnAsync(Loan loan);
}
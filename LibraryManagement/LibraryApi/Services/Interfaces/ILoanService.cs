using LibraryApi.DTOs.Loans;

namespace LibraryApi.Services.Interfaces;

public interface ILoanService
{
    Task<List<LoanDto>> GetAllLoansAsync();

    Task<LoanDto?> GetLoanByIdAsync(int id);

    Task<LoanDto> BorrowBookAsync(CreateLoanDto dto);

    Task<bool> ReturnBookAsync(int loanId);
}
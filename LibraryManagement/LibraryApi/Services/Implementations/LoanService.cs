using LibraryApi.DTOs.Loans;
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;

namespace LibraryApi.Services.Implementations;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;

    public LoanService(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<List<LoanDto>> GetAllLoansAsync()
    {
        var loans = await _loanRepository.GetAllAsync();

        return loans.Select(MapToDto).ToList();
    }

    public async Task<LoanDto?> GetLoanByIdAsync(int id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);

        return loan == null ? null : MapToDto(loan);
    }

    public async Task<LoanDto> BorrowBookAsync(CreateLoanDto dto)
    {
        var book = await _loanRepository.GetBookByIdAsync(dto.BookId);

        if (book == null)
            throw new InvalidOperationException("Livre introuvable.");

        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException(
                "Aucun exemplaire de ce livre n'est disponible.");

        var studentExists =
            await _loanRepository.StudentExistsAsync(dto.StudentId);

        if (!studentExists)
            throw new InvalidOperationException("Étudiant introuvable.");

        var borrowDate = DateTime.UtcNow;

        var loan = new Loan
        {
            BookId = dto.BookId,
            StudentId = dto.StudentId,
            BorrowDate = borrowDate,
            DueDate = borrowDate.AddDays(14),
            ReturnDate = null
        };

        book.AvailableCopies--;

        await _loanRepository.AddAsync(loan);

        var savedLoan = await _loanRepository.GetByIdAsync(loan.Id);

        return MapToDto(savedLoan!);
    }

    public async Task<bool> ReturnBookAsync(int loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
            return false;

        if (loan.ReturnDate.HasValue)
            throw new InvalidOperationException(
                "Ce livre a déjà été retourné.");

        if (loan.Book == null)
            throw new InvalidOperationException(
                "Le livre associé à cet emprunt est introuvable.");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Book.AvailableCopies++;

        await _loanRepository.SaveReturnAsync(loan);

        return true;
    }

    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            BookId = loan.BookId,
            BookTitle = loan.Book?.Title ?? string.Empty,
            StudentId = loan.StudentId,
            StudentName = loan.Student?.FullName ?? string.Empty,
            BorrowDate = loan.BorrowDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate
        };
    }
}
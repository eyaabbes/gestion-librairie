using LibraryApi.DTOs.Loans;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    // GET: api/loans
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var loans = await _loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    // GET: api/loans/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);

        if (loan == null)
            return NotFound(new { message = "Emprunt introuvable." });

        return Ok(loan);
    }

    // POST: api/loans
    [HttpPost]
    public async Task<IActionResult> BorrowBook(CreateLoanDto dto)
    {
        try
        {
            var loan = await _loanService.BorrowBookAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = loan.Id },
                loan
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/loans/5/return
    [HttpPut("{id}/return")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        try
        {
            var returned = await _loanService.ReturnBookAsync(id);

            if (!returned)
                return NotFound(new { message = "Emprunt introuvable." });

            return Ok(new { message = "Livre retourné avec succès." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
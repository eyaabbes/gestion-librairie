
using LibraryApi.Models;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    // GET: api/authors
    [HttpGet]
    public async Task<IActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        return Ok(authors);
    }

    // GET: api/authors/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAuthorById(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);

        if (author == null)
            return NotFound("Author not found.");

        return Ok(author);
    }

    // POST: api/authors
    [HttpPost]
    public async Task<IActionResult> AddAuthor(Author author)
    {
        await _authorService.AddAuthorAsync(author);

        return CreatedAtAction(
            nameof(GetAuthorById),
            new { id = author.Id },
            author
        );
    }

    // PUT: api/authors/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAuthor(
        int id, Author updatedAuthor)
    {
        var result = await _authorService.UpdateAuthorAsync(
            id, updatedAuthor);

        if (!result)
            return NotFound("Author not found.");

        return NoContent();
    }

    // DELETE: api/authors/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            var result = await _authorService.DeleteAuthorAsync(id);

            if (!result)
                return NotFound("Author not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

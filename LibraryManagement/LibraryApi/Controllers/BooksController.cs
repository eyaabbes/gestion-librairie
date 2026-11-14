
using LibraryApi.Models;
using LibraryApi.DTOs.Books;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: api/books
    [Authorize(Policy = "Books.View")]
    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    // GET: api/books/1
    [Authorize(Policy = "Books.View")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);

        if (book == null)
            return NotFound("Book not found.");

        return Ok(book);
    }

    // POST: api/books
    [Authorize(Policy = "Books.Create")]
    [HttpPost]
    public async Task<IActionResult> AddBook(BookInputDto book)
    {
        try
        {
            var newBook = await _bookService.AddBookAsync(book);

            return CreatedAtAction(
                nameof(GetBookById),
                new { id = newBook.Id },
                newBook
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/books/1
    [Authorize(Policy = "Books.Edit")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBook(int id, BookInputDto updatedBook)
    {
        try
        {
            var result = await _bookService.UpdateBookAsync(id, updatedBook);

            if (!result)
                return NotFound("Book not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/books/1/image (multipart/form-data, field: image)
    [Authorize(Policy = "Books.Edit")]
    [HttpPost("{id:int}/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadBookImage(
        int id,
        [FromForm(Name = "image")] IFormFile image)
    {
        try
        {
            var book = await _bookService.UploadBookImageAsync(id, image);

            if (book == null)
                return NotFound("Book not found.");

            return Ok(book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/books/1
    [Authorize(Policy = "Books.Delete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var result = await _bookService.DeleteBookAsync(id);

        if (!result)
            return NotFound("Book not found.");

        return NoContent();
    }
}

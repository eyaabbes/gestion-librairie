
using LibraryApi.Models;
using Microsoft.AspNetCore.Http;
using LibraryApi.DTOs.Books;

namespace LibraryApi.Services.Interfaces;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();

    Task<BookDto?> GetBookByIdAsync(int id);

    Task<BookDto> AddBookAsync(BookInputDto book);

    Task<bool> UpdateBookAsync(int id, BookInputDto updatedBook);

    Task<BookDto?> UploadBookImageAsync(int id, IFormFile image);

    Task<bool> DeleteBookAsync(int id);
}

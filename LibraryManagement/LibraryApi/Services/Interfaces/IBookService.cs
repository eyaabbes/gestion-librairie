
using LibraryApi.Models;

namespace LibraryApi.Services.Interfaces;

public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync();

    Task<Book?> GetBookByIdAsync(int id);

    Task<Book> AddBookAsync(Book book);

    Task<bool> UpdateBookAsync(int id, Book updatedBook);

    Task<bool> DeleteBookAsync(int id);
}


using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;

namespace LibraryApi.Services.Implementations;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        var existingBook =
            await _bookRepository.GetByISBNAsync(book.ISBN);

        if (existingBook != null)
            throw new InvalidOperationException("Cet ISBN existe déjà.");

        await _bookRepository.AddAsync(book);

        return book;
    }

    public async Task<bool> UpdateBookAsync(int id, Book updatedBook)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
            return false;

        var existingBook =
            await _bookRepository.GetByISBNAsync(updatedBook.ISBN);

        if (existingBook != null && existingBook.Id != id)
            throw new InvalidOperationException("Cet ISBN est déjà utilisé.");

        book.Title = updatedBook.Title;
        book.ISBN = updatedBook.ISBN;
        book.PublicationYear = updatedBook.PublicationYear;
        book.AvailableCopies = updatedBook.AvailableCopies;
        book.AuthorId = updatedBook.AuthorId;
        book.CategoryId = updatedBook.CategoryId;

        await _bookRepository.UpdateAsync(book);

        return true;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
            return false;

        await _bookRepository.DeleteAsync(book);

        return true;
    }
}

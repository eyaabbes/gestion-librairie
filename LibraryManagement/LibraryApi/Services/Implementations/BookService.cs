
using LibraryApi.Models;
using LibraryApi.DTOs.Books;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LibraryApi.Services.Implementations;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ISupabaseStorageService _storageService;

    public BookService(
        IBookRepository bookRepository,
        ISupabaseStorageService storageService)
    {
        _bookRepository = bookRepository;
        _storageService = storageService;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToDto).ToList();
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : MapToDto(book);
    }

    public async Task<BookDto> AddBookAsync(BookInputDto input)
    {
        var existingBook =
            await _bookRepository.GetByISBNAsync(input.ISBN);

        if (existingBook != null)
            throw new InvalidOperationException("Cet ISBN existe déjà.");

        var book = new Book
        {
            Title = input.Title,
            ISBN = input.ISBN,
            ImageUrl = input.ImageUrl,
            Price = input.Price,
            Description = input.Description,
            PublicationYear = input.PublicationYear,
            AvailableCopies = input.AvailableCopies,
            AuthorId = input.AuthorId,
            CategoryId = input.CategoryId
        };
        await _bookRepository.AddAsync(book);
        var created = await _bookRepository.GetByIdAsync(book.Id);
        return MapToDto(created ?? book);
    }

    public async Task<bool> UpdateBookAsync(int id, BookInputDto updatedBook)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
            return false;

        var existingBook =
            await _bookRepository.GetByISBNAsync(updatedBook.ISBN);

        if (existingBook != null && existingBook.Id != id)
            throw new InvalidOperationException("Cet ISBN est déjà utilisé.");

        var previousImageUrl = book.ImageUrl;
        book.Title = updatedBook.Title;
        book.ISBN = updatedBook.ISBN;
        book.ImageUrl = updatedBook.ImageUrl;
        book.Price = updatedBook.Price;
        book.Description = updatedBook.Description;
        book.PublicationYear = updatedBook.PublicationYear;
        book.AvailableCopies = updatedBook.AvailableCopies;
        book.AuthorId = updatedBook.AuthorId;
        book.CategoryId = updatedBook.CategoryId;

        await _bookRepository.UpdateAsync(book);

        if (!string.IsNullOrWhiteSpace(previousImageUrl)
            && !string.Equals(previousImageUrl, book.ImageUrl, StringComparison.Ordinal))
        {
            await _storageService.DeleteImageAsync(previousImageUrl);
        }

        return true;
    }

    public async Task<BookDto?> UploadBookImageAsync(int id, IFormFile image)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
            return null;

        var previousImageUrl = book.ImageUrl;
        var newImageUrl = await _storageService.UploadImageAsync(image);
        book.ImageUrl = newImageUrl;
        try
        {
            await _bookRepository.UpdateAsync(book);
        }
        catch
        {
            await _storageService.DeleteImageAsync(newImageUrl);
            throw;
        }

        if (!string.IsNullOrWhiteSpace(previousImageUrl)
            && !string.Equals(previousImageUrl, book.ImageUrl, StringComparison.Ordinal))
        {
            await _storageService.DeleteImageAsync(previousImageUrl);
        }

        var updatedBook = await _bookRepository.GetByIdAsync(id);
        return MapToDto(updatedBook ?? book);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
            return false;

        var imageUrl = book.ImageUrl;
        await _bookRepository.DeleteAsync(book);

        if (!string.IsNullOrWhiteSpace(imageUrl))
            await _storageService.DeleteImageAsync(imageUrl);

        return true;
    }

    private static BookDto MapToDto(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        ISBN = book.ISBN,
        ImageUrl = book.ImageUrl,
        Price = book.Price,
        Description = book.Description,
        PublicationYear = book.PublicationYear,
        AvailableCopies = book.AvailableCopies,
        AuthorId = book.AuthorId,
        Author = book.Author == null ? null : new BookAuthorDto
        {
            Id = book.Author.Id,
            FirstName = book.Author.FirstName,
            LastName = book.Author.LastName
        },
        CategoryId = book.CategoryId,
        Category = book.Category == null ? null : new BookCategoryDto
        {
            Id = book.Category.Id,
            Name = book.Category.Name
        }
    };
}

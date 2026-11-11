
using LibraryApi.Models;

namespace LibraryApi.Services.Interfaces;

public interface IAuthorService
{
    Task<List<Author>> GetAllAuthorsAsync();

    Task<Author?> GetAuthorByIdAsync(int id);

    Task AddAuthorAsync(Author author);

    Task<bool> UpdateAuthorAsync(int id, Author updatedAuthor);

    Task<bool> DeleteAuthorAsync(int id);
}

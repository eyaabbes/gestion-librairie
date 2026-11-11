
using LibraryApi.Models;

namespace LibraryApi.Repositories.Interfaces;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync();

    Task<Author?> GetByIdAsync(int id);

    Task AddAsync(Author author);

    Task UpdateAsync(Author author);

    Task DeleteAsync(Author author);

    Task<bool> HasBooksAsync(int authorId);
}

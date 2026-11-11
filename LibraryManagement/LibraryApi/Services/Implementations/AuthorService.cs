
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;

namespace LibraryApi.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<List<Author>> GetAllAuthorsAsync()
    {
        return await _authorRepository.GetAllAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(int id)
    {
        return await _authorRepository.GetByIdAsync(id);
    }

    public async Task AddAuthorAsync(Author author)
    {
        await _authorRepository.AddAsync(author);
    }

    public async Task<bool> UpdateAuthorAsync(
        int id, Author updatedAuthor)
    {
        var author = await _authorRepository.GetByIdAsync(id);

        if (author == null)
            return false;

        author.FirstName = updatedAuthor.FirstName;
        author.LastName = updatedAuthor.LastName;

        await _authorRepository.UpdateAsync(author);

        return true;
    }

    public async Task<bool> DeleteAuthorAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);

        if (author == null)
            return false;

        bool hasBooks = await _authorRepository.HasBooksAsync(id);

        if (hasBooks)
            throw new InvalidOperationException(
                "Impossible de supprimer cet auteur : il possède des livres."
            );

        await _authorRepository.DeleteAsync(author);

        return true;
    }
}

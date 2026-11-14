
using LibraryApi.Models;
using LibraryApi.DTOs.Authors;
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

    public async Task<List<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _authorRepository.GetAllAsync();
        return authors.Select(MapToDto).ToList();
    }

    public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        return author == null ? null : MapToDto(author);
    }

    public async Task<AuthorDto> AddAuthorAsync(AuthorInputDto input)
    {
        var author = new Author { FirstName = input.FirstName, LastName = input.LastName };
        await _authorRepository.AddAsync(author);
        return MapToDto(author);
    }

    public async Task<bool> UpdateAuthorAsync(
        int id, AuthorInputDto updatedAuthor)
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

    private static AuthorDto MapToDto(Author author) => new()
    {
        Id = author.Id,
        FirstName = author.FirstName,
        LastName = author.LastName
    };
}

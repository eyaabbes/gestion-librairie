
using LibraryApi.Models;
using LibraryApi.DTOs.Authors;

namespace LibraryApi.Services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAuthorsAsync();

    Task<AuthorDto?> GetAuthorByIdAsync(int id);

    Task<AuthorDto> AddAuthorAsync(AuthorInputDto author);

    Task<bool> UpdateAuthorAsync(int id, AuthorInputDto updatedAuthor);

    Task<bool> DeleteAuthorAsync(int id);
}

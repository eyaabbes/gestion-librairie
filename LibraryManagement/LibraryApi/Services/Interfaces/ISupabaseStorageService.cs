
using Microsoft.AspNetCore.Http;

namespace LibraryApi.Services.Interfaces;

public interface ISupabaseStorageService
{
    Task<string> UploadImageAsync(IFormFile file);

    Task DeleteImageAsync(string imageUrl);
}


using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LibraryApi.Services.Implementations;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SupabaseStorageService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Aucune image sélectionnée.");

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("L'image ne doit pas dépasser 5 MB.");

        var allowedTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException("Format d'image non autorisé.");

        var supabaseUrl = _configuration["Supabase:Url"]
            ?? throw new InvalidOperationException("Supabase URL manquante.");

        var secretKey = _configuration["Supabase:SecretKey"]
            ?? throw new InvalidOperationException("Supabase SecretKey manquante.");

        var bucket = _configuration["Supabase:Bucket"]
            ?? throw new InvalidOperationException("Supabase Bucket manquant.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Extension de fichier non autorisée.");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var storagePath = $"{bucket}/{fileName}";

        var url = $"{supabaseUrl.TrimEnd('/')}/storage/v1/object/{storagePath}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.TryAddWithoutValidation("apikey", secretKey);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", secretKey);

        request.Headers.TryAddWithoutValidation("x-upsert", "false");

        var content = new StreamContent(file.OpenReadStream());
        content.Headers.ContentType =
            new MediaTypeHeaderValue(file.ContentType);

        request.Content = content;

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Erreur Supabase : {error}");
        }

        return $"{supabaseUrl.TrimEnd('/')}/storage/v1/object/public/{storagePath}";
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        var supabaseUrl = _configuration["Supabase:Url"]
            ?? throw new InvalidOperationException("Supabase URL manquante.");
        var secretKey = _configuration["Supabase:SecretKey"]
            ?? throw new InvalidOperationException("Supabase SecretKey manquante.");
        var bucket = _configuration["Supabase:Bucket"]
            ?? throw new InvalidOperationException("Supabase Bucket manquant.");

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri)
            || !Uri.TryCreate(supabaseUrl, UriKind.Absolute, out var baseUri)
            || !string.Equals(imageUri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var pathSegments = imageUri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(Uri.UnescapeDataString)
            .ToArray();

        // Only remove public objects belonging to the configured bucket.
        if (pathSegments.Length < 6
            || pathSegments[0] != "storage"
            || pathSegments[1] != "v1"
            || pathSegments[2] != "object"
            || pathSegments[3] != "public"
            || pathSegments[4] != bucket)
        {
            return;
        }

        var objectPath = string.Join('/', pathSegments.Skip(5));
        if (string.IsNullOrWhiteSpace(objectPath))
            return;

        var deleteUrl = $"{supabaseUrl.TrimEnd('/')}/storage/v1/object/{Uri.EscapeDataString(bucket)}";
        using var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl)
        {
            Content = JsonContent.Create(new { prefixes = new[] { objectPath } })
        };

        request.Headers.TryAddWithoutValidation("apikey", secretKey);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", secretKey);

        using var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Erreur Supabase : {error}");
        }
    }
}

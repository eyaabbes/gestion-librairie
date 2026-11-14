namespace LibraryApi.DTOs.Books;

public sealed class BookInputDto
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public int PublicationYear { get; set; }
    public int AvailableCopies { get; set; }
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
}

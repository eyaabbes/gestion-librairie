namespace LibraryApi.DTOs.Books;

public sealed class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public int PublicationYear { get; set; }
    public int AvailableCopies { get; set; }
    public int AuthorId { get; set; }
    public BookAuthorDto? Author { get; set; }
    public int CategoryId { get; set; }
    public BookCategoryDto? Category { get; set; }
}

public sealed class BookAuthorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public sealed class BookCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

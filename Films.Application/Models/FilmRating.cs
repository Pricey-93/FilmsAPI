namespace Films.Application.Models;

public class FilmRating
{
    public required Guid FilmId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
}

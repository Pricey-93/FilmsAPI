namespace Films.Contracts.Responses;

public record FilmRatingResponse
{
    public required Guid FilmId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
}
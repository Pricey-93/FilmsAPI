namespace Films.Contracts.Responses;

public record FilmResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int YearOfRelease { get; init; }
    public required IEnumerable<string> Genres { get; init; } = [];
}
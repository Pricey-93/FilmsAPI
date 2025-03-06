namespace Films.Contracts.Responses;

public record FilmsResponse
{
    public required IEnumerable<FilmResponse> Items { get; init; } = [];
}
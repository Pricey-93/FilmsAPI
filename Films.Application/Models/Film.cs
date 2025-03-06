namespace Films.Application.Models;

public class Film
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required int YearOfRelease { get; set; }
    public required List<string> Genres { get; init; } = [];
}
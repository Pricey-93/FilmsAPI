using Films.Application.Models;

namespace Films.Application.Interfaces.Services;

public interface IFilmService
{
    Task<bool> CreateAsync(Film film, CancellationToken cToken = default);
    Task<Film?> GetByIdAsync(Guid id, Guid? userId, CancellationToken cToken = default);
    Task<Film?> GetBySlugAsync(string slug, Guid? userId, CancellationToken cToken = default);
    Task<IEnumerable<Film>> GetAllAsync(Guid? userId, CancellationToken cToken = default);
    Task<Film?> UpdateAsync(Film film, Guid? userId, CancellationToken cToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cToken = default);
    Task<bool> ExistsAsync(Guid id);
}
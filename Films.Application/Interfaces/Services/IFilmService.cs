using Films.Application.Models;

namespace Films.Application.Interfaces.Services;

public interface IFilmService
{
    Task<bool> CreateAsync(Film film);
    Task<Film?> GetByIdAsync(Guid id);
    Task<Film?> GetBySlugAsync(string slug);
    Task<IEnumerable<Film>> GetAllAsync();
    Task<Film?> UpdateAsync(Film film);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
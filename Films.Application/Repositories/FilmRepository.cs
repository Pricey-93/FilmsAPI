using Films.Application.Interfaces;
using Films.Application.Models;

namespace Films.Application.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly List<Film> _films = [];
    
    public Task<bool> CreateAsync(Film film)
    {
        _films.Add(film);
        return Task.FromResult(true);
    }

    public Task<Film?> GetByIdAsync(Guid id)
    {
        var film = _films.FirstOrDefault(f => f.Id == id);
        return Task.FromResult(film);
    }

    public Task<IEnumerable<Film>> GetAllAsync()
    {
        return Task.FromResult(_films.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Film film)
    {
        var filmIndex = _films.FindIndex(f => f.Id == film.Id);
        if (filmIndex == -1)
        {
            return Task.FromResult(false);
        }
        
        _films[filmIndex] = film;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removedCount = _films.RemoveAll(f => f.Id == id);
        var filmRemoved = removedCount > 0;
        return Task.FromResult(filmRemoved);
    }
}
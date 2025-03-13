using Films.Application.Interfaces.Repositories;
using Films.Application.Interfaces.Services;
using Films.Application.Models;
using FluentValidation;

namespace Films.Application.Services;

public class FilmService : IFilmService
{
    private readonly IFilmRepository _filmRepository;
    private readonly IValidator<Film> _filmValidator;

    public FilmService(IFilmRepository filmRepository, IValidator<Film> filmValidator)
    {
        _filmRepository = filmRepository;
        _filmValidator = filmValidator;
    }

    public async Task<bool> CreateAsync(Film film)
    {
        await _filmValidator.ValidateAndThrowAsync(film);
        
        return await _filmRepository.CreateAsync(film);
    }

    public Task<Film?> GetByIdAsync(Guid id)
    {
        return _filmRepository.GetByIdAsync(id);
    }

    public Task<Film?> GetBySlugAsync(string slug)
    {
        return _filmRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Film>> GetAllAsync()
    {
        return _filmRepository.GetAllAsync();
    }

    public async Task<Film?> UpdateAsync(Film film)
    {
        await _filmValidator.ValidateAndThrowAsync(film);
        
        var exists = await ExistsAsync(film.Id);

        if (!exists)
        {
            return null;
        }
        
        await _filmRepository.UpdateAsync(film);
        return film;
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _filmRepository.DeleteAsync(id);
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _filmRepository.ExistsAsync(id);
    }
}
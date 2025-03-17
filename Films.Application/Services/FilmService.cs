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

    public async Task<bool> CreateAsync(Film film, CancellationToken cToken)
    {
        await _filmValidator.ValidateAndThrowAsync(film, cancellationToken: cToken);
        
        return await _filmRepository.CreateAsync(film, cToken);
    }

    public Task<Film?> GetByIdAsync(Guid id, CancellationToken cToken)
    {
        return _filmRepository.GetByIdAsync(id, cToken);
    }

    public Task<Film?> GetBySlugAsync(string slug, CancellationToken cToken)
    {
        return _filmRepository.GetBySlugAsync(slug, cToken);
    }

    public Task<IEnumerable<Film>> GetAllAsync(CancellationToken cToken)
    {
        return _filmRepository.GetAllAsync(cToken);
    }

    public async Task<Film?> UpdateAsync(Film film, CancellationToken cToken)
    {
        await _filmValidator.ValidateAndThrowAsync(film, cancellationToken: cToken);
        
        var exists = await ExistsAsync(film.Id);

        if (!exists)
        {
            return null;
        }
        
        await _filmRepository.UpdateAsync(film, cToken);
        return film;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cToken)
    {
        return _filmRepository.DeleteAsync(id, cToken);
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _filmRepository.ExistsAsync(id);
    }
}
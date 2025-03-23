using Films.Application.Interfaces.Repositories;
using Films.Application.Interfaces.Services;
using Films.Application.Models;
using FluentValidation;

namespace Films.Application.Services;

public class FilmService : IFilmService
{
    private readonly IFilmRepository _filmRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<Film> _filmValidator;

    public FilmService(IFilmRepository filmRepository, IRatingRepository ratingRepository, IValidator<Film> filmValidator)
    {
        _filmRepository = filmRepository;
        _ratingRepository = ratingRepository;
        _filmValidator = filmValidator;
    }

    public async Task<bool> CreateAsync(Film film, CancellationToken cToken)
    {
        await _filmValidator.ValidateAndThrowAsync(film, cancellationToken: cToken);
        
        return await _filmRepository.CreateAsync(film, cToken);
    }

    public Task<Film?> GetByIdAsync(Guid id, Guid? userId, CancellationToken cToken)
    {
        return _filmRepository.GetByIdAsync(id, userId, cToken);
    }

    public Task<Film?> GetBySlugAsync(string slug, Guid? userId, CancellationToken cToken)
    {
        return _filmRepository.GetBySlugAsync(slug, userId, cToken);
    }

    public Task<IEnumerable<Film>> GetAllAsync(Guid? userId, CancellationToken cToken)
    {
        return _filmRepository.GetAllAsync(userId, cToken);
    }

    public async Task<Film?> UpdateAsync(Film film, Guid? userId, CancellationToken cToken)
    {
        await _filmValidator.ValidateAndThrowAsync(film, cancellationToken: cToken);
        
        var exists = await ExistsAsync(film.Id);

        if (!exists)
        {
            return null;
        }
        
        await _filmRepository.UpdateAsync(film, cToken);

        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(film.Id, cToken);
            film.Rating = rating;
            return film;
        }
        var ratings = await _ratingRepository.GetRatingAsync(film.Id, userId.Value, cToken);
        
        film.Rating = ratings.Rating;
        film.UserRating = ratings.UserRating;
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
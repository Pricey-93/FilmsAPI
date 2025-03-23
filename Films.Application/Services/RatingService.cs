using Films.Application.Interfaces.Repositories;
using Films.Application.Interfaces.Services;
using Films.Application.Models;
using FluentValidation;
using FluentValidation.Results;

namespace Films.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IFilmRepository _filmRepository;

    public RatingService(IRatingRepository ratingRepository, IFilmRepository filmRepository)
    {
        _ratingRepository = ratingRepository;
        _filmRepository = filmRepository;
    }

    public async Task<bool> RateFilmAsync(Guid filmId, int rating, Guid userId, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            });
        }

        var filmExists = await _filmRepository.ExistsAsync(filmId);
        if (!filmExists)
        {
            return false;
        }

        return await _ratingRepository.RateFilmAsync(filmId, rating, userId, token);
    }

    public Task<bool> DeleteRatingAsync(Guid filmId, Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.DeleteRatingAsync(filmId, userId, token);
    }

    public Task<IEnumerable<FilmRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, token);
    }
}



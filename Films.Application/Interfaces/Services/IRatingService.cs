using Films.Application.Models;

namespace Films.Application.Interfaces.Services;

public interface IRatingService
{
    Task<bool> RateFilmAsync(Guid filmId, int rating, Guid userId, CancellationToken token = default);

    Task<bool> DeleteRatingAsync(Guid filmId, Guid userId, CancellationToken token = default);

    Task<IEnumerable<FilmRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}

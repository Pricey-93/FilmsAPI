using Films.Application.Models;

namespace Films.Application. Interfaces.Repositories;

public interface IRatingRepository
{
    Task<bool> RateFilmAsync(Guid filmId, int rating, Guid userId, CancellationToken token = default);

    Task<float?> GetRatingAsync(Guid filmId, CancellationToken token = default);
    
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid filmId, Guid userId, CancellationToken token = default);

    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);

    Task<IEnumerable<FilmRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}




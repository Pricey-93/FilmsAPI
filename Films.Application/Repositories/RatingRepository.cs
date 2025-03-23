using Dapper;
using Films.Application.Database;
using Films.Application.Interfaces.Repositories;
using Films.Application.Models;

namespace Films.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateFilmAsync(Guid filmId, int rating, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into rating(user_id, film_id, rating) 
            values (@userId, @filmId, @rating)
            on conflict (user_id, film_id) do update 
                set rating = @rating
            """, new { userId, filmId, rating }, cancellationToken: token));

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid filmId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select round(avg(r.rating), 1) from rating r
            where film_id = @filmId
            """, new { filmId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid filmId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(rating), 1), 
                   (select rating 
                    from rating 
                    where film_id = @filmId 
                      and user_id = @userId
                    limit 1) 
            from rating
            where film_id = @filmId
            """, new { filmId, userId }, cancellationToken: token));
    }

    public async Task<bool> DeleteRatingAsync(Guid filmId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from rating
            where film_id = @filmId
            and user_id = @userId
            """, new { userId, filmId }, cancellationToken: token));

        return result > 0;
    }

    public async Task<IEnumerable<FilmRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryAsync<FilmRating>(new CommandDefinition("""
            select r.rating, r.film_id, f.slug
            from rating r
            inner join film f on r.film_id = f.id
            where user_id = @userId
            """, new { userId }, cancellationToken: token));
    }
}




using Dapper;
using Films.Application.Models;
using Films.Application.Database;
using Films.Application.Interfaces.Repositories;

namespace Films.Application.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    public FilmRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<bool> CreateAsync(Film film, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into film (id, slug, title, year_of_release)
                                                                             values (@Id, @Slug, @Title, @YearOfRelease)
                                                                         """, film, cancellationToken: cToken));
        if (result > 0)
        {
            foreach (var genre in film.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    insert into genre (film_id, name)
                                                                        values (@Id, @Name)
                                                                    """, new { Id = film.Id, Name = genre }, cancellationToken: cToken));
            }
        }
        transaction.Commit();
            
        return result > 0;
    }

    public async Task<Film?> GetByIdAsync(Guid id, Guid? userId, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        
        var film = await connection.QuerySingleOrDefaultAsync<Film>(
            new CommandDefinition("""
                                  select f.*, round(avg(r.rating), 1) as rating, myr.rating as user_rating
                                  from film f 
                                  left join rating r on f.id = r.film_id
                                  left join rating myr on f.id = myr.film_id
                                    and myr.user_id = @UserId
                                  where id = @id
                                  group by id, user_rating
                                  """, new { id, userId }, cancellationToken: cToken));

        if (film is null)
        {
            return null;
        }
        
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("select name from genre where film_id = @id", new { id }, cancellationToken: cToken)
            );

        foreach (var genre in genres)
        {
            film.Genres.Add(genre);
        }
        
        return film;
    }
    
    public async Task<Film?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        
        var film = await connection.QuerySingleOrDefaultAsync<Film>(
            new CommandDefinition("""
                                  select f.*, round(avg(r.rating), 1) as rating, myr.rating as user_rating
                                  from film f 
                                  left join rating r on f.id = r.film_id
                                  left join rating myr on f.id = myr.film_id
                                    and myr.user_id = @UserId
                                  where slug = @Slug
                                  group by id, user_rating
                                  """, new { slug, userId }, cancellationToken: cToken));

        if (film is null)
        {
            return null;
        }
        
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("select name from genre where film_id = @id", new { id = film.Id }, cancellationToken: cToken)
        );

        foreach (var genre in genres)
        {
            film.Genres.Add(genre);
        }
        
        return film;
    }

    public async Task<IEnumerable<Film>> GetAllAsync(Guid? userId, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var films = await connection.QueryAsync(
            new CommandDefinition("""
                                  select f.*,
                                      string_agg(distinct g.name, ',') as genres,
                                      round(avg(r.rating), 1) as rating,
                                      myr.rating as user_rating
                                  from film f 
                                  left join genre g on f.id = g.film_id
                                  left join rating r on f.id = r.film_id
                                  left join rating myr on f.id = myr.film_id
                                    and myr.user_id = @UserId
                                  group by id, user_rating
                                  """, new { userId }, cancellationToken: cToken));

        return films.Select(x => new Film
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.year_of_release,
            Rating = (float?)x.rating,
            UserRating = (int?)x.user_rating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Film film, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genre where film_id = @Id
                                                            """, new { id = film.Id }, cancellationToken: cToken));

        foreach (var genre in film.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genre (film_id, name)
                                                                values (@Id, @Name)
                                                                """, new { Id = film.Id, Name = genre }, cancellationToken: cToken));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update film set slug = @Slug, title = @Title, year_of_release = @YearOfRelease
                                                                         where id = @Id
                                                                         """, film, cancellationToken: cToken));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genre where film_id = @Id
                                                            """, new { id }, cancellationToken: cToken));
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from film where id = @Id
                                                            """, new { id }, cancellationToken: cToken));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from film where id = @id
                                                                               """, new { id }));
    }
}
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
    
    public async Task<bool> CreateAsync(Film film)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into film (id, slug, title, year_of_release)
                                                                             values (@Id, @Slug, @Title, @YearOfRelease)
                                                                         """, film));
        if (result > 0)
        {
            foreach (var genre in film.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    insert into genre (film_id, name)
                                                                        values (@Id, @Name)
                                                                    """, new { Id = film.Id, Name = genre }));
            }
        }
        transaction.Commit();
            
        return result > 0;
    }

    public async Task<Film?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var film = await connection.QuerySingleOrDefaultAsync<Film>(
            new CommandDefinition("select * from film where id = @id", id)
            );

        if (film is null)
        {
            return null;
        }
        
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("select name from genre where film_id = @id", id)
            );

        foreach (var genre in genres)
        {
            film.Genres.Add(genre);
        }
        
        return film;
    }
    
    public async Task<Film?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var film = await connection.QuerySingleOrDefaultAsync<Film>(
            new CommandDefinition("select * from film where slug = @slug", slug)
        );

        if (film is null)
        {
            return null;
        }
        
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("select name from genre where film_id = @id", film.Id)
        );

        foreach (var genre in genres)
        {
            film.Genres.Add(genre);
        }
        
        return film;
    }

    public async Task<IEnumerable<Film>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var films = await connection.QueryAsync(
            new CommandDefinition("""
                                  select f.*,
                                  string_agg(g.name, ',') as genres
                                  from film f left join genre g on f.id = g.film_id
                                  group by id
                                  """));

        return films.Select(x => new Film
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.year_of_release,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Film film)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genre where film_id = @Id
                                                            """, film.Id));

        foreach (var genre in film.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genre (film_id, name)
                                                                values (@filmId, @Name)
                                                                """, new { filmId = film.Id, name = genre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update film set slug = @Slug, title = @Title, year_of_release = @YearOfRelease
                                                                         where id = @Id
                                                                         """, film));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genre where film_id = @Id
                                                            """, id));
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from film where id = @Id
                                                            """, id));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from film where id = @id
                                                                               """, id));
    }
}
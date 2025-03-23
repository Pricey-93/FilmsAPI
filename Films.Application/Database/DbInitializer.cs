using Dapper;

namespace Films.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                                      create table if not exists film (
                                          id UUID primary key,
                                          slug TEXT not null,
                                          title TEXT not null,
                                          year_of_release integer not null);
                                      """);
        
        await connection.ExecuteAsync("""
                                      create unique index concurrently if not exists film_slug_idx
                                      on film
                                      using btree(slug);
                                      """);

        await connection.ExecuteAsync("""
                                      create table if not exists genre (
                                          film_id UUID references film(id),
                                          name TEXT not null);
                                      """);

        await connection.ExecuteAsync("""
                                      create table if not exists rating (
                                          user_id UUID,
                                          film_id uuid references film(id),
                                          rating integer not null,
                                          primary key (user_id, film_id));
                                      """);
    }
}
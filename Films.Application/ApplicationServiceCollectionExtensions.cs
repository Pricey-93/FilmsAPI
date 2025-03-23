using Microsoft.Extensions.DependencyInjection;
using Films.Application.Database;
using Films.Application.Interfaces.Repositories;
using Films.Application.Interfaces.Services;
using Films.Application.Repositories;
using Films.Application.Services;
using FluentValidation;


namespace Films.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IFilmRepository, FilmRepository>();
        services.AddSingleton<IFilmService, FilmService>();
        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IRatingService, RatingService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}
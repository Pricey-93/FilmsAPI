using Microsoft.Extensions.DependencyInjection;
using Films.Application.Interfaces;
using Films.Application.Repositories;


namespace Films.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IFilmRepository, FilmRepository>();
        return services;
    }
}
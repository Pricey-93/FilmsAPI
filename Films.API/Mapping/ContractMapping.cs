using Films.Application.Models;
using Films.Contracts.Requests;
using Films.Contracts.Responses;

namespace Films.API.Mapping;

public static class ContractMapping
{
    public static Film MapToFilm(this CreateFilmRequest request)
    {
        return new Film
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList(),
        };
    }

    public static FilmResponse MapToResponse(this Film film)
    {
        return new FilmResponse
        {
            Id = film.Id,
            Title = film.Title,
            Slug = film.Slug,
            Rating = film.Rating,
            UserRating = film.UserRating,
            YearOfRelease = film.YearOfRelease,
            Genres = film.Genres,
        };
    }
    
    public static FilmsResponse MapToResponse(this IEnumerable<Film> films)
    {
        return new FilmsResponse
        {
            Items = films.Select(MapToResponse)
        };
    }
    
    public static Film MapToFilm(this UpdateFilmRequest request, Guid id)
    {
        return new Film
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList(),
        };
    }
    
    public static IEnumerable<FilmRatingResponse> MapToResponse(this IEnumerable<FilmRating> ratings)
    {
        return ratings.Select(x => new FilmRatingResponse
        {
            Rating = x.Rating,
            Slug = x.Slug,
            FilmId = x.FilmId
        });
    }
}
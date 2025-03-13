using Films.Application.Interfaces.Repositories;
using Films.Application.Interfaces.Services;
using Films.Application.Models;
using FluentValidation;

namespace Films.Application.Validators;

public class FilmValidator : AbstractValidator<Film>
{
    private readonly IFilmRepository _filmRepository;
    
    public FilmValidator(IFilmRepository filmRepository)
    {
        _filmRepository = filmRepository;
        
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This film already exists in the system");
    }

    private async Task<bool> ValidateSlug(Film film, string slug, CancellationToken cancellationToken)
    {
        var existingFilm = await _filmRepository.GetBySlugAsync(slug);

        if (existingFilm is not null)
        {
            return existingFilm.Id == film.Id;
        }
        
        return existingFilm is null;
    }
}
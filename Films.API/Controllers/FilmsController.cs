using Films.API.Mapping;
using Films.Application.Interfaces;
using Films.Contracts.Requests;
using Films.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Films.API.Controllers;

[ApiController]
public class FilmsController : ControllerBase
{
    private readonly IFilmRepository _filmRepository;

    public FilmsController(IFilmRepository filmRepository)
    {
        _filmRepository = filmRepository;
    }

    [HttpPost(ApiEndpoints.Films.Create)]
    public async Task<IActionResult> Create([FromBody] CreateFilmRequest request)
    {
        var film = request.MapToFilm();
        await _filmRepository.CreateAsync(film);
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = film.Id }, film);
    }

    [HttpGet(ApiEndpoints.Films.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var film = Guid.TryParse(idOrSlug, out var id) 
            ? await _filmRepository.GetByIdAsync(id)
            : await _filmRepository.GetBySlugAsync(slug: idOrSlug);
        
        if (film is null)
        {
            return NotFound();
        }

        return Ok(film.MapToResponse());
    }
    
    [HttpGet(ApiEndpoints.Films.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var films = await _filmRepository.GetAllAsync();

        return Ok(films.MapToResponse());
    }

    [HttpPut(ApiEndpoints.Films.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateFilmRequest request)
    {
        var film = request.MapToFilm(id);
        var updated = await _filmRepository.UpdateAsync(film);
        if (!updated)
        {
            return NotFound();
        }
        
        var response = film.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Films.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _filmRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
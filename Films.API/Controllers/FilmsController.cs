using Films.API.Mapping;
using Films.Application.Interfaces;
using Films.Application.Interfaces.Repositories;
using Films.Application.Interfaces.Services;
using Films.Contracts.Requests;
using Films.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Films.API.Controllers;

[ApiController]
public class FilmsController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmsController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    [HttpPost(ApiEndpoints.Films.Create)]
    public async Task<IActionResult> Create([FromBody] CreateFilmRequest request)
    {
        var film = request.MapToFilm();
        await _filmService.CreateAsync(film);
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = film.Id }, film);
    }

    [HttpGet(ApiEndpoints.Films.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var film = Guid.TryParse(idOrSlug, out var id) 
            ? await _filmService.GetByIdAsync(id)
            : await _filmService.GetBySlugAsync(idOrSlug);
        
        if (film is null)
        {
            return NotFound();
        }

        return Ok(film.MapToResponse());
    }
    
    [HttpGet(ApiEndpoints.Films.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var films = await _filmService.GetAllAsync();

        return Ok(films.MapToResponse());
    }

    [HttpPut(ApiEndpoints.Films.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateFilmRequest request)
    {
        var film = request.MapToFilm(id);
        var updated = await _filmService.UpdateAsync(film);
        if (updated is null)
        {
            return NotFound();
        }
        
        var response = updated.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Films.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _filmService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
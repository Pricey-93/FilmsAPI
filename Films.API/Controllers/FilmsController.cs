using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Films.API.Auth;
using Films.API.Mapping;
using Films.Application.Interfaces.Services;
using Films.Contracts.Requests;

namespace Films.API.Controllers;

[ApiController]
public class FilmsController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmsController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Films.Create)]
    public async Task<IActionResult> Create([FromBody] CreateFilmRequest request, CancellationToken cToken)
    {
        foreach (var genre in request.Genres)
        {
            Console.WriteLine($"Genre in controller method: {genre}");
        }
        var film = request.MapToFilm();
        await _filmService.CreateAsync(film, cToken);
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = film.Id }, film);
    }

    [HttpGet(ApiEndpoints.Films.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken cToken)
    {
        var userId = HttpContext.GetUserId();
        
        var film = Guid.TryParse(idOrSlug, out var id) 
            ? await _filmService.GetByIdAsync(id, userId, cToken)
            : await _filmService.GetBySlugAsync(idOrSlug, userId, cToken);
        
        if (film is null)
        {
            return NotFound();
        }

        return Ok(film.MapToResponse());
    }
    
    [HttpGet(ApiEndpoints.Films.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cToken)
    {
        var userId = HttpContext.GetUserId();
        
        var films = await _filmService.GetAllAsync(userId, cToken);

        return Ok(films.MapToResponse());
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Films.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateFilmRequest request, CancellationToken cToken)
    {
        var userId = HttpContext.GetUserId();
        
        var film = request.MapToFilm(id);
        var updated = await _filmService.UpdateAsync(film, userId, cToken);
        if (updated is null)
        {
            return NotFound();
        }
        
        var response = updated.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Films.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cToken)
    {
        var deleted = await _filmService.DeleteAsync(id, cToken);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
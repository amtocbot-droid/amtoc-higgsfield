using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Application.Features.Tutorials;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HiggsfieldTutorial.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutorialsController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TutorialSummaryDto>>> GetAll(
        [FromQuery] string? category, [FromQuery] string? search, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTutorialsQuery(category, search), ct);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<TutorialDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTutorialBySlugQuery(slug), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TutorialDto>> Create(CreateTutorialRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateTutorialCommand(request), ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }
}

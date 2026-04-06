using HiggsfieldTutorial.Application.Features.Prompts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HiggsfieldTutorial.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromptsController(IMediator _mediator) : ControllerBase
{
    [HttpGet("examples")]
    public async Task<ActionResult> GetExamples([FromQuery] string? category, [FromQuery] bool featured = false, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPromptExamplesQuery(category, featured), ct);
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPromptCategoriesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("models")]
    public async Task<ActionResult> GetModels(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAvailableModelsQuery(), ct);
        return Ok(result);
    }
}

using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Application.Features.Generation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HiggsfieldTutorial.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerationController(IMediator _mediator) : ControllerBase
{
    [HttpPost("submit")]
    public async Task<ActionResult<GenerationJobDto>> Submit([FromBody] SubmitGenerationRequest request, CancellationToken ct)
    {
        var apiKey = Request.Headers["X-Higgsfield-Key"].FirstOrDefault() ?? "";
        var apiSecret = Request.Headers["X-Higgsfield-Secret"].FirstOrDefault() ?? "";
        var userId = Guid.TryParse(Request.Headers["X-User-Id"].FirstOrDefault(), out var uid) ? uid : Guid.Empty;

        var result = await _mediator.Send(new SubmitGenerationCommand(request, userId, apiKey, apiSecret), ct);
        return Accepted(result);
    }

    [HttpGet("{jobId:guid}/status")]
    public async Task<ActionResult<GenerationStatusDto>> GetStatus(Guid jobId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGenerationStatusQuery(jobId), ct);
        return Ok(result);
    }

    [HttpPost("{jobId:guid}/cancel")]
    public async Task<ActionResult> Cancel(Guid jobId, CancellationToken ct)
    {
        var apiKey = Request.Headers["X-Higgsfield-Key"].FirstOrDefault() ?? "";
        var apiSecret = Request.Headers["X-Higgsfield-Secret"].FirstOrDefault() ?? "";
        var cancelled = await _mediator.Send(new CancelGenerationCommand(jobId, apiKey, apiSecret), ct);
        return cancelled ? Accepted() : BadRequest("Cannot cancel — job already processing");
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> Webhook([FromBody] WebhookNotification notification, CancellationToken ct)
    {
        await _mediator.Send(notification, ct);
        return Ok();
    }
}

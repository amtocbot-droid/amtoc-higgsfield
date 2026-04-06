using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Application.Features.Automation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HiggsfieldTutorial.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutomationController(IMediator _mediator) : ControllerBase
{
    [HttpPost("connect")]
    public async Task<ActionResult<bool>> Connect([FromBody] AutomationConnectRequest request, CancellationToken ct)
    {
        var connected = await _mediator.Send(new ConnectBrowserCommand(request.CdpUrl), ct);
        return connected
            ? Ok(new { connected = true })
            : BadRequest(new { connected = false, error = "Could not connect to browser. Make sure Chrome is running with --remote-debugging-port=9222 --remote-debugging-address=0.0.0.0" });
    }

    [HttpPost("disconnect")]
    public async Task<ActionResult> Disconnect(CancellationToken ct)
    {
        await _mediator.Send(new DisconnectBrowserCommand(), ct);
        return Ok();
    }

    [HttpGet("status")]
    public async Task<ActionResult<AutomationStatusDto>> GetStatus(CancellationToken ct)
    {
        var status = await _mediator.Send(new GetAutomationStatusQuery(), ct);
        return Ok(status);
    }

    [HttpPost("generate-image")]
    public async Task<ActionResult<AutomationJobDto>> GenerateImage([FromBody] GenerateImageAutomationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerateImageAutomationCommand(request), ct);
        return Accepted(result);
    }

    [HttpPost("generate-video")]
    public async Task<ActionResult<AutomationJobDto>> GenerateVideo([FromBody] GenerateVideoAutomationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerateVideoAutomationCommand(request), ct);
        return Accepted(result);
    }

    [HttpPost("generate-cinema")]
    public async Task<ActionResult<AutomationJobDto>> GenerateCinema([FromBody] GenerateCinemaAutomationRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerateCinemaAutomationCommand(request), ct);
        return Accepted(result);
    }
}

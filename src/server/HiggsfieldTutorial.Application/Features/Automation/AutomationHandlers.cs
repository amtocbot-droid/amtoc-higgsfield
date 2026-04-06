using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Domain.Interfaces;
using MediatR;

namespace HiggsfieldTutorial.Application.Features.Automation;

public record ConnectBrowserCommand(string CdpUrl) : IRequest<bool>;

public record DisconnectBrowserCommand : IRequest<Unit>;

public record GetAutomationStatusQuery : IRequest<AutomationStatusDto>;

public record GenerateImageAutomationCommand(GenerateImageAutomationRequest Request) : IRequest<AutomationJobDto>;

public record GenerateVideoAutomationCommand(GenerateVideoAutomationRequest Request) : IRequest<AutomationJobDto>;

public record GenerateCinemaAutomationCommand(GenerateCinemaAutomationRequest Request) : IRequest<AutomationJobDto>;

public class AutomationHandlers :
    IRequestHandler<ConnectBrowserCommand, bool>,
    IRequestHandler<DisconnectBrowserCommand, Unit>,
    IRequestHandler<GetAutomationStatusQuery, AutomationStatusDto>,
    IRequestHandler<GenerateImageAutomationCommand, AutomationJobDto>,
    IRequestHandler<GenerateVideoAutomationCommand, AutomationJobDto>,
    IRequestHandler<GenerateCinemaAutomationCommand, AutomationJobDto>
{
    private readonly IBrowserAutomationService _automation;

    public AutomationHandlers(IBrowserAutomationService automation)
    {
        _automation = automation;
    }

    public async Task<bool> Handle(ConnectBrowserCommand cmd, CancellationToken ct)
    {
        return await _automation.ConnectAsync(cmd.CdpUrl, ct);
    }

    public async Task<Unit> Handle(DisconnectBrowserCommand cmd, CancellationToken ct)
    {
        await _automation.DisconnectAsync();
        return Unit.Value;
    }

    public Task<AutomationStatusDto> Handle(GetAutomationStatusQuery query, CancellationToken ct)
    {
        return Task.FromResult(new AutomationStatusDto(
            _automation.IsConnected,
            _automation.CurrentJobId,
            _automation.CurrentJobStatus,
            null
        ));
    }

    public async Task<AutomationJobDto> Handle(GenerateImageAutomationCommand cmd, CancellationToken ct)
    {
        var result = await _automation.GenerateImageAsync(
            cmd.Request.Model, cmd.Request.Prompt,
            cmd.Request.AspectRatio, cmd.Request.ImageUrl, ct);

        return new AutomationJobDto(
            result.JobId, result.Mode, result.Status,
            result.ResultUrl, result.ErrorMessage, result.ResultUrls);
    }

    public async Task<AutomationJobDto> Handle(GenerateVideoAutomationCommand cmd, CancellationToken ct)
    {
        var result = await _automation.GenerateVideoAsync(
            cmd.Request.Model, cmd.Request.Prompt,
            cmd.Request.AspectRatio, cmd.Request.Duration,
            cmd.Request.ImageUrl, ct);

        return new AutomationJobDto(
            result.JobId, result.Mode, result.Status,
            result.ResultUrl, result.ErrorMessage, result.ResultUrls);
    }

    public async Task<AutomationJobDto> Handle(GenerateCinemaAutomationCommand cmd, CancellationToken ct)
    {
        var shots = cmd.Request.Shots
            .Select(s => (s.Prompt, s.DurationSeconds, s.ImageUrl))
            .ToList();

        var result = await _automation.GenerateCinemaAsync(shots, ct);

        return new AutomationJobDto(
            result.JobId, result.Mode, result.Status,
            result.ResultUrl, result.ErrorMessage, result.ResultUrls);
    }
}

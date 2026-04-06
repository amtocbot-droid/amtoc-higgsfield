using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Application.Interfaces;
using HiggsfieldTutorial.Domain.Entities;
using HiggsfieldTutorial.Domain.Interfaces;
using MediatR;

namespace HiggsfieldTutorial.Application.Features.Generation;

public record SubmitGenerationCommand(SubmitGenerationRequest Request, Guid UserId, string ApiKey, string ApiSecret) : IRequest<GenerationJobDto>;

public record GetGenerationStatusQuery(Guid JobId) : IRequest<GenerationStatusDto>;

public record CancelGenerationCommand(Guid JobId, string ApiKey, string ApiSecret) : IRequest<bool>;

public record WebhookNotification(string RequestId, string Status, string? ImageUrl, string? VideoUrl, string? ErrorMessage) : IRequest<bool>;

public class GenerationHandlers(
    IGenerationJobRepository _jobs,
    IHiggsfieldApiClient _higgsfield) :
    IRequestHandler<SubmitGenerationCommand, GenerationJobDto>,
    IRequestHandler<GetGenerationStatusQuery, GenerationStatusDto>,
    IRequestHandler<CancelGenerationCommand, bool>,
    IRequestHandler<WebhookNotification, bool>
{
    public async Task<GenerationJobDto> Handle(SubmitGenerationCommand cmd, CancellationToken ct)
    {
        var job = new GenerationJob
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            ModelId = cmd.Request.ModelId,
            Prompt = cmd.Request.Prompt,
            ImageUrl = cmd.Request.ImageUrl,
            MediaType = cmd.Request.MediaType,
            Status = "queued",
            ParametersJson = System.Text.Json.JsonSerializer.Serialize(cmd.Request),
            CreatedAt = DateTime.UtcNow
        };

        await _jobs.AddAsync(job, ct);

        try
        {
            var result = await _higgsfield.SubmitAsync(cmd.Request, cmd.ApiKey, cmd.ApiSecret, ct);
            job.HiggsfieldRequestId = result.RequestId;
            await _jobs.UpdateAsync(job, ct);
        }
        catch (HttpRequestException ex)
        {
            job.Status = "failed";
            job.ErrorMessage = ex.Message;
            await _jobs.UpdateAsync(job, ct);
        }

        return new GenerationJobDto(job.Id, job.ModelId, job.Prompt, job.Status, null, job.MediaType, job.CreatedAt, null);
    }

    public async Task<GenerationStatusDto> Handle(GetGenerationStatusQuery query, CancellationToken ct)
    {
        var job = await _jobs.GetByIdAsync(query.JobId, ct)
            ?? throw new KeyNotFoundException($"Job {query.JobId} not found");

        return new GenerationStatusDto(
            job.HiggsfieldRequestId ?? job.Id.ToString(),
            job.Status,
            job.ResultUrl,
            job.ErrorMessage
        );
    }

    public async Task<bool> Handle(CancelGenerationCommand cmd, CancellationToken ct)
    {
        var job = await _jobs.GetByIdAsync(cmd.JobId, ct)
            ?? throw new KeyNotFoundException($"Job {cmd.JobId} not found");

        if (job.Status != "queued") return false;

        var cancelled = await _higgsfield.CancelAsync(job.HiggsfieldRequestId!, cmd.ApiKey, cmd.ApiSecret, ct);
        if (cancelled)
        {
            job.Status = "cancelled";
            await _jobs.UpdateAsync(job, ct);
        }
        return cancelled;
    }

    public async Task<bool> Handle(WebhookNotification notification, CancellationToken ct)
    {
        var job = await _jobs.GetByHiggsfieldRequestIdAsync(notification.RequestId, ct);
        if (job is null) return false;

        job.Status = notification.Status;
        job.ResultUrl = notification.ImageUrl ?? notification.VideoUrl;
        job.ErrorMessage = notification.ErrorMessage;

        if (notification.Status is "completed" or "failed" or "nsfw")
            job.CompletedAt = DateTime.UtcNow;

        await _jobs.UpdateAsync(job, ct);
        return true;
    }
}

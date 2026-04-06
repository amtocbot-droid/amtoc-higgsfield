using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Domain.Interfaces;
using MediatR;

namespace HiggsfieldTutorial.Application.Features.Tutorials;

public record GetTutorialsQuery(string? Category, string? Search) : IRequest<IReadOnlyList<TutorialSummaryDto>>;

public record GetTutorialBySlugQuery(string Slug) : IRequest<TutorialDto?>;

public record CreateTutorialCommand(CreateTutorialRequest Request) : IRequest<TutorialDto>;

public class TutorialHandlers(ITutorialRepository _tutorials) :
    IRequestHandler<GetTutorialsQuery, IReadOnlyList<TutorialSummaryDto>>,
    IRequestHandler<GetTutorialBySlugQuery, TutorialDto?>,
    IRequestHandler<CreateTutorialCommand, TutorialDto>
{
    public async Task<IReadOnlyList<TutorialSummaryDto>> Handle(GetTutorialsQuery request, CancellationToken ct)
    {
        var tutorials = !string.IsNullOrWhiteSpace(request.Search)
            ? await _tutorials.SearchAsync(request.Search, ct)
            : request.Category is not null
                ? await _tutorials.GetByCategoryAsync(request.Category, ct)
                : await _tutorials.GetPublishedAsync(ct);

        return tutorials.Select(t => new TutorialSummaryDto(
            t.Id, t.Title, t.Slug, t.Summary, t.Category, t.DifficultyLevel,
            t.EstimatedMinutes, t.CoverImageUrl, t.Tags
        )).ToList();
    }

    public async Task<TutorialDto?> Handle(GetTutorialBySlugQuery request, CancellationToken ct)
    {
        var tutorial = await _tutorials.GetBySlugAsync(request.Slug, ct);
        if (tutorial is null) return null;

        return new TutorialDto(
            tutorial.Id, tutorial.Title, tutorial.Slug, tutorial.Summary,
            tutorial.Category, tutorial.DifficultyLevel, tutorial.EstimatedMinutes,
            tutorial.CoverImageUrl, tutorial.Tags,
            tutorial.Steps.Select(s => new TutorialStepDto(
                s.StepNumber, s.Title, s.Instruction, s.CodeSnippet,
                s.ExpectedResult, s.ScreenshotUrl, s.Tip
            )).ToList(),
            tutorial.Resources.Select(r => new TutorialResourceDto(
                r.Label, r.Url, r.ResourceType
            )).ToList()
        );
    }

    public async Task<TutorialDto> Handle(CreateTutorialCommand request, CancellationToken ct)
    {
        var r = request.Request;
        var tutorial = new Domain.Entities.Tutorial
        {
            Id = Guid.NewGuid(),
            Title = r.Title,
            Slug = r.Title.ToLowerInvariant().Replace(' ', '-'),
            Summary = r.Summary,
            Content = r.Content,
            Category = r.Category,
            DifficultyLevel = r.DifficultyLevel,
            EstimatedMinutes = r.EstimatedMinutes,
            SortOrder = r.SortOrder,
            CoverImageUrl = r.CoverImageUrl,
            Tags = r.Tags,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            Steps = r.Steps.Select(s => new Domain.Entities.TutorialStep
            {
                Id = Guid.NewGuid(),
                StepNumber = s.StepNumber,
                Title = s.Title,
                Instruction = s.Instruction,
                CodeSnippet = s.CodeSnippet,
                ExpectedResult = s.ExpectedResult,
                ScreenshotUrl = s.ScreenshotUrl,
                Tip = s.Tip
            }).ToList()
        };

        await _tutorials.AddAsync(tutorial, ct);
        return await Handle(new GetTutorialBySlugQuery(tutorial.Slug), ct) ?? throw new InvalidOperationException();
    }
}

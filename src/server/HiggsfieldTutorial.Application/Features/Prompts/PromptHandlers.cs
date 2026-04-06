using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Domain.Interfaces;
using MediatR;

namespace HiggsfieldTutorial.Application.Features.Prompts;

public record GetPromptExamplesQuery(string? Category, bool FeaturedOnly) : IRequest<IReadOnlyList<PromptExampleDto>>;

public record GetPromptCategoriesQuery : IRequest<IReadOnlyList<PromptCategoryDto>>;

public record GetAvailableModelsQuery : IRequest<IReadOnlyList<ModelInfoDto>>;

public class PromptHandlers(IPromptExampleRepository _prompts) :
    IRequestHandler<GetPromptExamplesQuery, IReadOnlyList<PromptExampleDto>>,
    IRequestHandler<GetPromptCategoriesQuery, IReadOnlyList<PromptCategoryDto>>,
    IRequestHandler<GetAvailableModelsQuery, IReadOnlyList<ModelInfoDto>>
{
    public async Task<IReadOnlyList<PromptExampleDto>> Handle(GetPromptExamplesQuery request, CancellationToken ct)
    {
        var examples = request.FeaturedOnly
            ? await _prompts.GetFeaturedAsync(ct)
            : request.Category is not null
                ? await _prompts.GetByCategoryAsync(request.Category, ct)
                : await _prompts.GetAllAsync(ct);

        return examples.Select(e => new PromptExampleDto(
            e.Id, e.Title, e.Prompt, e.Category, e.MediaType,
            e.ResultImageUrl, e.ModelId, e.Tags, e.IsFeatured, e.Upvotes
        )).ToList();
    }

    public async Task<IReadOnlyList<PromptCategoryDto>> Handle(GetPromptCategoriesQuery request, CancellationToken ct)
    {
        var all = await _prompts.GetAllAsync(ct);
        return all.GroupBy(p => p.Category)
            .Select(g => new PromptCategoryDto(g.Key, g.Count()))
            .ToList();
    }

    public async Task<IReadOnlyList<ModelInfoDto>> Handle(GetAvailableModelsQuery request, CancellationToken ct)
    {
        await Task.CompletedTask;
        return
        [
            new("bytedance/seedream/v4/text-to-image", "Seedream 4.0", "image", "Advanced text-to-image (verified)", true, ["text-to-image"]),
            new("higgsfield-ai/soul/standard", "Soul", "image", "Flagship text-to-image model", true, ["text-to-image"]),
        ];
    }
}

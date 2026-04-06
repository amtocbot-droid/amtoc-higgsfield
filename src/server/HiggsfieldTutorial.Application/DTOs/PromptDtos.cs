namespace HiggsfieldTutorial.Application.DTOs;

public record PromptExampleDto(
    Guid Id,
    string Title,
    string Prompt,
    string Category,
    string MediaType,
    string? ResultImageUrl,
    string ModelId,
    string[] Tags,
    bool IsFeatured,
    int Upvotes
);

public record PromptCategoryDto(
    string Name,
    int Count
);

public record ModelInfoDto(
    string ModelId,
    string Name,
    string MediaType,
    string Description,
    bool IsFree,
    string[] Capabilities
);

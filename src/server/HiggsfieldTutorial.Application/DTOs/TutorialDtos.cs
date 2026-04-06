namespace HiggsfieldTutorial.Application.DTOs;

public record TutorialDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string Category,
    string DifficultyLevel,
    int EstimatedMinutes,
    string? CoverImageUrl,
    string[] Tags,
    List<TutorialStepDto> Steps,
    List<TutorialResourceDto> Resources
);

public record TutorialSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string Category,
    string DifficultyLevel,
    int EstimatedMinutes,
    string? CoverImageUrl,
    string[] Tags
);

public record TutorialStepDto(
    int StepNumber,
    string Title,
    string Instruction,
    string? CodeSnippet,
    string? ExpectedResult,
    string? ScreenshotUrl,
    string? Tip
);

public record TutorialResourceDto(
    string Label,
    string Url,
    string ResourceType
);

public record CreateTutorialRequest(
    string Title,
    string Summary,
    string Content,
    string Category,
    string DifficultyLevel,
    int EstimatedMinutes,
    int SortOrder,
    string? CoverImageUrl,
    string[] Tags,
    List<CreateTutorialStepRequest> Steps
);

public record CreateTutorialStepRequest(
    int StepNumber,
    string Title,
    string Instruction,
    string? CodeSnippet,
    string? ExpectedResult,
    string? ScreenshotUrl,
    string? Tip
);

namespace HiggsfieldTutorial.Application.DTOs;

public record SubmitGenerationRequest(
    string ModelId,
    string Prompt,
    string MediaType,
    string? ImageUrl,
    string? AspectRatio,
    string? Resolution,
    int? Duration,
    string? WebhookUrl
);

public record GenerationJobDto(
    Guid Id,
    string ModelId,
    string Prompt,
    string Status,
    string? ResultUrl,
    string MediaType,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

public record GenerationStatusDto(
    string RequestId,
    string Status,
    string? ResultUrl,
    string? ErrorMessage
);

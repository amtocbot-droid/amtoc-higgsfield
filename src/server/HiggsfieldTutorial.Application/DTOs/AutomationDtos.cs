namespace HiggsfieldTutorial.Application.DTOs;

public record AutomationConnectRequest(string CdpUrl = "http://localhost:9222");

public record AutomationStatusDto(
    bool IsConnected,
    string? CurrentJobId,
    string? CurrentJobStatus,
    string? CurrentJobMode
);

public record GenerateImageAutomationRequest(
    string Model,
    string Prompt,
    string? AspectRatio,
    string? ImageUrl
);

public record GenerateVideoAutomationRequest(
    string Model,
    string Prompt,
    string? AspectRatio,
    int? Duration,
    string? ImageUrl
);

public record GenerateCinemaAutomationRequest(
    List<CinemaShotDto> Shots
);

public record CinemaShotDto(
    string Prompt,
    int DurationSeconds,
    string? ImageUrl
);

public record AutomationJobDto(
    string JobId,
    string Mode,
    string Status,
    string? ResultUrl,
    string? ErrorMessage,
    List<string>? ResultUrls
);

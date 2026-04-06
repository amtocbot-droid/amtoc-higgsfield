using HiggsfieldTutorial.Domain.Entities;

namespace HiggsfieldTutorial.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ITutorialRepository : IRepository<Tutorial>
{
    Task<IReadOnlyList<Tutorial>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<Tutorial?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IReadOnlyList<Tutorial>> GetPublishedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Tutorial>> SearchAsync(string query, CancellationToken ct = default);
}

public interface IGenerationJobRepository : IRepository<GenerationJob>
{
    Task<IReadOnlyList<GenerationJob>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<GenerationJob?> GetByHiggsfieldRequestIdAsync(string requestId, CancellationToken ct = default);
}

public interface IPromptExampleRepository : IRepository<PromptExample>
{
    Task<IReadOnlyList<PromptExample>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<IReadOnlyList<PromptExample>> GetFeaturedAsync(CancellationToken ct = default);
}

public interface IBrowserAutomationService : IDisposable
{
    Task<bool> ConnectAsync(string cdpUrl, CancellationToken ct = default);
    Task DisconnectAsync();
    bool IsConnected { get; }
    string? CurrentJobId { get; }
    string? CurrentJobStatus { get; }

    Task<BrowserAutomationResult> GenerateImageAsync(string model, string prompt, string? aspectRatio, string? imageUrl, CancellationToken ct = default);
    Task<BrowserAutomationResult> GenerateVideoAsync(string model, string prompt, string? aspectRatio, int? duration, string? imageUrl, CancellationToken ct = default);
    Task<BrowserAutomationResult> GenerateCinemaAsync(List<(string Prompt, int DurationSeconds, string? ImageUrl)> shots, CancellationToken ct = default);
}

public record BrowserAutomationResult(
    string JobId,
    string Mode,
    string Status,
    string? ResultUrl,
    string? ErrorMessage,
    List<string>? ResultUrls
);

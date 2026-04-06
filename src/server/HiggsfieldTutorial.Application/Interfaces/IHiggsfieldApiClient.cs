using HiggsfieldTutorial.Application.DTOs;

namespace HiggsfieldTutorial.Application.Interfaces;

public interface IHiggsfieldApiClient
{
    Task<HiggsfieldSubmitResponse> SubmitAsync(SubmitGenerationRequest request, string apiKey, string apiSecret, CancellationToken ct = default);
    Task<HiggsfieldStatusResponse> GetStatusAsync(string requestId, string apiKey, string apiSecret, CancellationToken ct = default);
    Task<bool> CancelAsync(string requestId, string apiKey, string apiSecret, CancellationToken ct = default);
}

public record HiggsfieldSubmitResponse(string RequestId, string Status, string StatusUrl, string CancelUrl);
public record HiggsfieldStatusResponse(string RequestId, string Status, string? ImageUrl, string? VideoUrl, string? ErrorMessage);

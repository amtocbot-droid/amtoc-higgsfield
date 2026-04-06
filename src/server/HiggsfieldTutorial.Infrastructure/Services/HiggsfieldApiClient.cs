using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HiggsfieldTutorial.Application.DTOs;
using HiggsfieldTutorial.Application.Interfaces;

namespace HiggsfieldTutorial.Infrastructure.Services;

public class HiggsfieldApiClient(HttpClient _httpClient) : IHiggsfieldApiClient
{
    public async Task<HiggsfieldSubmitResponse> SubmitAsync(SubmitGenerationRequest request, string apiKey, string apiSecret, CancellationToken ct = default)
    {
        var payload = new Dictionary<string, object>();
        payload["prompt"] = request.Prompt;

        if (request.ImageUrl is not null) payload["image_url"] = request.ImageUrl;
        if (request.AspectRatio is not null) payload["aspect_ratio"] = request.AspectRatio;
        if (request.Resolution is not null) payload["resolution"] = request.Resolution;
        if (request.Duration is not null) payload["duration"] = request.Duration.Value;

        var url = $"https://platform.higgsfield.ai/{request.ModelId}";
        if (request.WebhookUrl is not null)
            url += $"?hf_webhook={Uri.EscapeDataString(request.WebhookUrl)}";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Key", $"{apiKey}:{apiSecret}");
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(req, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Higgsfield API error {(int)response.StatusCode}: {errorBody}");
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        var doc = JsonDocument.Parse(json);

        return new HiggsfieldSubmitResponse(
            doc.RootElement.GetProperty("request_id").GetString()!,
            doc.RootElement.GetProperty("status").GetString()!,
            doc.RootElement.TryGetProperty("status_url", out var su) ? su.GetString()! : "",
            doc.RootElement.TryGetProperty("cancel_url", out var cu) ? cu.GetString()! : ""
        );
    }

    public async Task<HiggsfieldStatusResponse> GetStatusAsync(string requestId, string apiKey, string apiSecret, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"https://platform.higgsfield.ai/requests/{requestId}/status");
        req.Headers.Authorization = new AuthenticationHeaderValue("Key", $"{apiKey}:{apiSecret}");

        using var response = await _httpClient.SendAsync(req, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var doc = JsonDocument.Parse(json);
        var status = doc.RootElement.GetProperty("status").GetString()!;

        string? imageUrl = null;
        string? videoUrl = null;
        string? errorMsg = null;

        if (doc.RootElement.TryGetProperty("images", out var images) && images.GetArrayLength() > 0)
            imageUrl = images[0].GetProperty("url").GetString();

        if (doc.RootElement.TryGetProperty("video", out var video))
            videoUrl = video.GetProperty("url").GetString();

        if (doc.RootElement.TryGetProperty("error", out var error))
            errorMsg = error.GetString();

        return new HiggsfieldStatusResponse(requestId, status, imageUrl, videoUrl, errorMsg);
    }

    public async Task<bool> CancelAsync(string requestId, string apiKey, string apiSecret, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"https://platform.higgsfield.ai/requests/{requestId}/cancel");
        req.Headers.Authorization = new AuthenticationHeaderValue("Key", $"{apiKey}:{apiSecret}");

        using var response = await _httpClient.SendAsync(req, ct);
        return response.StatusCode == System.Net.HttpStatusCode.Accepted;
    }
}

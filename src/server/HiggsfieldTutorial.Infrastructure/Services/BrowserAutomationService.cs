using System.Collections.Concurrent;
using HiggsfieldTutorial.Domain.Interfaces;
using Microsoft.Playwright;

namespace HiggsfieldTutorial.Infrastructure.Services;

public class BrowserAutomationService : IBrowserAutomationService
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private bool _disposed;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ConcurrentDictionary<string, BrowserAutomationResult> _jobs = new();

    public bool IsConnected => _browser?.IsConnected ?? false;
    public string? CurrentJobId { get; private set; }
    public string? CurrentJobStatus { get; private set; }

    public async Task<bool> ConnectAsync(string cdpUrl, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            if (_browser?.IsConnected == true)
                return true;

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.ConnectOverCDPAsync(cdpUrl, new()
            {
                Timeout = 15000
            });

            var defaultContext = _browser.Contexts.FirstOrDefault();
            _page = defaultContext?.Pages.FirstOrDefault();

            return _browser.IsConnected;
        }
        catch
        {
            _playwright?.Dispose();
            _playwright = null;
            _browser = null;
            _page = null;
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DisconnectAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_browser?.IsConnected == true)
            {
                try { await _browser.CloseAsync(); } catch { }
            }
            _browser = null;
            _page = null;
            _playwright?.Dispose();
            _playwright = null;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<BrowserAutomationResult> GenerateImageAsync(
        string model, string prompt, string? aspectRatio, string? imageUrl,
        CancellationToken ct = default)
    {
        var jobId = Guid.NewGuid().ToString("N")[..12];
        CurrentJobId = jobId;
        CurrentJobStatus = "running";
        var result = new BrowserAutomationResult(jobId, "image", "running", null, null, null);
        _jobs[jobId] = result;

        await _lock.WaitAsync(ct);
        try
        {
            var page = await EnsurePageAsync();
            var route = string.IsNullOrEmpty(model) ? "/image" : $"/image/{model}";
            await page.GotoAsync($"https://higgsfield.ai{route}",
                new() { WaitUntil = WaitUntilState.NetworkIdle });
            await page.WaitForTimeoutAsync(2000);

            await FillPromptAndSubmitAsync(page, prompt, aspectRatio, imageUrl, ct);
            var resultUrl = await WaitForResultAsync(page, ct);

            CurrentJobStatus = resultUrl != null ? "completed" : "failed";
            result = result with
            {
                Status = CurrentJobStatus,
                ResultUrl = resultUrl,
                ErrorMessage = resultUrl == null ? "Could not detect generated result" : null
            };
            _jobs[jobId] = result;
            return result;
        }
        catch (Exception ex)
        {
            CurrentJobStatus = "failed";
            result = result with { Status = "failed", ErrorMessage = ex.Message };
            _jobs[jobId] = result;
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<BrowserAutomationResult> GenerateVideoAsync(
        string model, string prompt, string? aspectRatio, int? duration, string? imageUrl,
        CancellationToken ct = default)
    {
        var jobId = Guid.NewGuid().ToString("N")[..12];
        CurrentJobId = jobId;
        CurrentJobStatus = "running";
        var result = new BrowserAutomationResult(jobId, "video", "running", null, null, null);
        _jobs[jobId] = result;

        await _lock.WaitAsync(ct);
        try
        {
            var page = await EnsurePageAsync();
            var queryParams = string.IsNullOrEmpty(model) ? "" : $"?model={model}";
            await page.GotoAsync($"https://higgsfield.ai/create/video{queryParams}",
                new() { WaitUntil = WaitUntilState.NetworkIdle });
            await page.WaitForTimeoutAsync(2000);

            await FillPromptAndSubmitAsync(page, prompt, aspectRatio, imageUrl, ct);
            var resultUrl = await WaitForResultAsync(page, ct, timeout: 300000);

            CurrentJobStatus = resultUrl != null ? "completed" : "failed";
            result = result with
            {
                Status = CurrentJobStatus,
                ResultUrl = resultUrl,
                ErrorMessage = resultUrl == null ? "Could not detect generated result" : null
            };
            _jobs[jobId] = result;
            return result;
        }
        catch (Exception ex)
        {
            CurrentJobStatus = "failed";
            result = result with { Status = "failed", ErrorMessage = ex.Message };
            _jobs[jobId] = result;
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<BrowserAutomationResult> GenerateCinemaAsync(
        List<(string Prompt, int DurationSeconds, string? ImageUrl)> shots,
        CancellationToken ct = default)
    {
        var jobId = Guid.NewGuid().ToString("N")[..12];
        CurrentJobId = jobId;
        CurrentJobStatus = "running";
        var result = new BrowserAutomationResult(jobId, "cinema", "running", null, null, null);
        _jobs[jobId] = result;

        await _lock.WaitAsync(ct);
        try
        {
            var page = await EnsurePageAsync();
            await page.GotoAsync("https://higgsfield.ai/cinema-studio",
                new() { WaitUntil = WaitUntilState.NetworkIdle });
            await page.WaitForTimeoutAsync(3000);

            var resultUrls = new List<string>();

            for (var i = 0; i < shots.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                var shot = shots[i];

                try
                {
                    var textarea = page.Locator("textarea").First;
                    await textarea.WaitForAsync(new()
                        { State = WaitForSelectorState.Visible, Timeout = 15000 });
                    await textarea.FillAsync(shot.Prompt);
                    await page.WaitForTimeoutAsync(500);

                    var generateBtn = page.Locator(
                        "button:has-text(\"Generate\"), button:has-text(\"Create\"), button[type='submit']").First;
                    await generateBtn.ClickAsync();

                    var shotResultUrl = await WaitForResultAsync(page, ct, timeout: 180000);
                    if (shotResultUrl != null)
                        resultUrls.Add(shotResultUrl);
                }
                catch (Exception ex)
                {
                    resultUrls.Add($"error: {ex.Message}");
                }
            }

            CurrentJobStatus = resultUrls.Count > 0 ? "completed" : "failed";
            result = result with
            {
                Status = CurrentJobStatus,
                ResultUrls = resultUrls,
                ErrorMessage = resultUrls.Count == 0 ? "No shots completed" : null
            };
            _jobs[jobId] = result;
            return result;
        }
        catch (Exception ex)
        {
            CurrentJobStatus = "failed";
            result = result with { Status = "failed", ErrorMessage = ex.Message };
            _jobs[jobId] = result;
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try { _browser?.CloseAsync().GetAwaiter().GetResult(); } catch { }
        _playwright?.Dispose();
        _lock.Dispose();
    }

    private async Task<IPage> EnsurePageAsync()
    {
        if (_page == null || !_browser?.IsConnected == true)
            throw new InvalidOperationException("Browser not connected. Call ConnectAsync first.");

        if (_page.IsClosed)
        {
            var context = _browser!.Contexts.FirstOrDefault()
                ?? throw new InvalidOperationException("No browser context available.");
            _page = context.Pages.FirstOrDefault() ?? await context.NewPageAsync();
        }

        return _page;
    }

    private async Task FillPromptAndSubmitAsync(
        IPage page, string prompt, string? aspectRatio, string? imageUrl,
        CancellationToken ct)
    {
        var textarea = page.Locator("textarea").First;
        await textarea.WaitForAsync(new()
            { State = WaitForSelectorState.Visible, Timeout = 15000 });
        await textarea.FillAsync(prompt);

        if (!string.IsNullOrEmpty(aspectRatio))
        {
            try
            {
                var aspectButton = page.Locator(
                    "button:has-text(\"aspect\"), [aria-label*=\"aspect\"], [aria-label*=\"Aspect\"]").First;
                if (await aspectButton.IsVisibleAsync())
                {
                    await aspectButton.ClickAsync();
                    await page.WaitForTimeoutAsync(500);
                    var option = page.Locator(
                        $"[role='option']:has-text(\"{aspectRatio}\"), button:has-text(\"{aspectRatio}\")").First;
                    if (await option.IsVisibleAsync())
                        await option.ClickAsync();
                }
            }
            catch { }
        }

        if (!string.IsNullOrEmpty(imageUrl))
        {
            try
            {
                var fileInput = page.Locator("input[type='file']").First;
                if (await fileInput.IsVisibleAsync())
                {
                    await fileInput.SetInputFilesAsync(imageUrl);
                    await page.WaitForTimeoutAsync(1000);
                }
            }
            catch { }
        }

        var generateBtn = page.Locator(
            "button:has-text(\"Generate\"), button:has-text(\"Create\"), button[type='submit']").First;
        await generateBtn.ClickAsync();
    }

    private async Task<string?> WaitForResultAsync(IPage page, CancellationToken ct, int timeout = 120000)
    {
        var startTime = DateTime.UtcNow;
        var timeoutSpan = TimeSpan.FromMilliseconds(timeout);

        while (DateTime.UtcNow - startTime < timeoutSpan)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                var resultImage = page.Locator("img[src*='cdn'], img[src*='higgsfield']").Last;
                if (await resultImage.IsVisibleAsync())
                {
                    var src = await resultImage.GetAttributeAsync("src");
                    if (!string.IsNullOrEmpty(src) && !src.Contains("avatar") && !src.Contains("logo"))
                        return src;
                }

                var resultVideo = page.Locator("video source, video[src]").Last;
                if (await resultVideo.IsVisibleAsync())
                {
                    var src = await resultVideo.GetAttributeAsync("src");
                    if (!string.IsNullOrEmpty(src))
                        return src;
                }

                var downloadLink = page.Locator("a[download], a:has-text(\"Download\")").Last;
                if (await downloadLink.IsVisibleAsync())
                {
                    var href = await downloadLink.GetAttributeAsync("href");
                    if (!string.IsNullOrEmpty(href))
                        return href;
                }
            }
            catch { }

            await page.WaitForTimeoutAsync(3000);
        }

        return null;
    }
}

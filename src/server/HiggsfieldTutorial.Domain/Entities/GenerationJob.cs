namespace HiggsfieldTutorial.Domain.Entities;

public class GenerationJob
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ModelId { get; set; } = default!;
    public string Prompt { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "queued";
    public string? HiggsfieldRequestId { get; set; }
    public string? ResultUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string MediaType { get; set; } = default!;
    public string ParametersJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

namespace HiggsfieldTutorial.Domain.Entities;

public class PromptExample
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Prompt { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string MediaType { get; set; } = default!;
    public string? ResultImageUrl { get; set; }
    public string ModelId { get; set; } = default!;
    public string[] Tags { get; set; } = [];
    public bool IsFeatured { get; set; }
    public int Upvotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

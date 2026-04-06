namespace HiggsfieldTutorial.Domain.Entities;

public class Tutorial
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string DifficultyLevel { get; set; } = default!;
    public int EstimatedMinutes { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; }
    public string? CoverImageUrl { get; set; }
    public string[] Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TutorialStep> Steps { get; set; } = [];
    public ICollection<TutorialResource> Resources { get; set; } = [];
}

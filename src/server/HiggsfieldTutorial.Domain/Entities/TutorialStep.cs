namespace HiggsfieldTutorial.Domain.Entities;

public class TutorialStep
{
    public Guid Id { get; set; }
    public Guid TutorialId { get; set; }
    public int StepNumber { get; set; }
    public string Title { get; set; } = default!;
    public string Instruction { get; set; } = default!;
    public string? CodeSnippet { get; set; }
    public string? ExpectedResult { get; set; }
    public string? ScreenshotUrl { get; set; }
    public string? Tip { get; set; }

    public Tutorial Tutorial { get; set; } = default!;
}

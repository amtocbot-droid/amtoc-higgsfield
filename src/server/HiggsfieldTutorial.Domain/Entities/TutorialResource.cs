namespace HiggsfieldTutorial.Domain.Entities;

public class TutorialResource
{
    public Guid Id { get; set; }
    public Guid TutorialId { get; set; }
    public string Label { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string ResourceType { get; set; } = default!;

    public Tutorial Tutorial { get; set; } = default!;
}

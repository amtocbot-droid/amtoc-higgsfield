namespace HiggsfieldTutorial.Domain.Enums;

public enum MediaType
{
    Image,
    Video,
    Audio
}

public enum GenerationStatus
{
    Queued,
    InProgress,
    Completed,
    Failed,
    NSFW,
    Cancelled
}

public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced
}

public enum TutorialCategory
{
    GettingStarted,
    ImageGeneration,
    VideoGeneration,
    VisualEffects,
    CinemaStudio,
    CharacterCreation,
    Apps,
    APIIntegration,
    PromptEngineering,
    Advanced
}

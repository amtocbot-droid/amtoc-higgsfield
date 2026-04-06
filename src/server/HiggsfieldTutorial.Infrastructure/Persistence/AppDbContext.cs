using HiggsfieldTutorial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HiggsfieldTutorial.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tutorial> Tutorials => Set<Tutorial>();
    public DbSet<TutorialStep> TutorialSteps => Set<TutorialStep>();
    public DbSet<TutorialResource> TutorialResources => Set<TutorialResource>();
    public DbSet<GenerationJob> GenerationJobs => Set<GenerationJob>();
    public DbSet<PromptExample> PromptExamples => Set<PromptExample>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tutorial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Tags).HasColumnType("jsonb");
            entity.HasMany(e => e.Steps).WithOne(s => s.Tutorial).HasForeignKey(s => s.TutorialId);
            entity.HasMany(e => e.Resources).WithOne(r => r.Tutorial).HasForeignKey(r => r.TutorialId);
        });

        modelBuilder.Entity<TutorialStep>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TutorialResource>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<GenerationJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.HiggsfieldRequestId);
        });

        modelBuilder.Entity<PromptExample>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tags).HasColumnType("jsonb");
        });

        modelBuilder.Entity<Tutorial>().HasData(SeedData.Tutorials);
        modelBuilder.Entity<TutorialStep>().HasData(SeedData.Steps);
        modelBuilder.Entity<PromptExample>().HasData(SeedData.PromptExamples);
    }
}

file static class SeedData
{
    private static readonly DateTime SeedDate = new(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly Tutorial[] Tutorials =
    [
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Title = "Getting Started with Higgsfield", Slug = "getting-started", Summary = "Learn the basics of Higgsfield AI platform", Content = "", Category = "GettingStarted", DifficultyLevel = "Beginner", EstimatedMinutes = 10, SortOrder = 1, IsPublished = true, CreatedAt = SeedDate, Tags = ["intro", "basics"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Title = "Image Generation Basics", Slug = "image-generation-basics", Summary = "Create stunning images using text prompts", Content = "", Category = "ImageGeneration", DifficultyLevel = "Beginner", EstimatedMinutes = 15, SortOrder = 2, IsPublished = true, CreatedAt = SeedDate, Tags = ["images", "text-to-image"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Title = "Video Generation from Images", Slug = "video-generation-images", Summary = "Transform static images into dynamic videos", Content = "", Category = "VideoGeneration", DifficultyLevel = "Beginner", EstimatedMinutes = 20, SortOrder = 3, IsPublished = true, CreatedAt = SeedDate, Tags = ["video", "image-to-video"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Title = "Prompt Engineering for AI Art", Slug = "prompt-engineering", Summary = "Master the art of writing effective prompts", Content = "", Category = "PromptEngineering", DifficultyLevel = "Intermediate", EstimatedMinutes = 25, SortOrder = 4, IsPublished = true, CreatedAt = SeedDate, Tags = ["prompts", "advanced"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Title = "Soul ID Character Creation", Slug = "soul-id-character", Summary = "Create unique AI characters with Soul ID", Content = "", Category = "CharacterCreation", DifficultyLevel = "Intermediate", EstimatedMinutes = 15, SortOrder = 5, IsPublished = true, CreatedAt = SeedDate, Tags = ["character", "soul-id"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Title = "Cinema Studio Masterclass", Slug = "cinema-studio", Summary = "Director-level control over your AI videos", Content = "", Category = "CinemaStudio", DifficultyLevel = "Advanced", EstimatedMinutes = 30, SortOrder = 6, IsPublished = true, CreatedAt = SeedDate, Tags = ["cinema", "studio", "directing"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Title = "Visual Effects Library", Slug = "visual-effects", Summary = "Apply 80+ VFX presets to your content", Content = "", Category = "VisualEffects", DifficultyLevel = "Intermediate", EstimatedMinutes = 20, SortOrder = 7, IsPublished = true, CreatedAt = SeedDate, Tags = ["vfx", "effects", "transitions"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Title = "Using Higgsfield Apps", Slug = "higgsfield-apps", Summary = "One-click content creation with 60+ apps", Content = "", Category = "Apps", DifficultyLevel = "Beginner", EstimatedMinutes = 15, SortOrder = 8, IsPublished = true, CreatedAt = SeedDate, Tags = ["apps", "one-click"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Title = "API Quick Start Guide", Slug = "api-quick-start", Summary = "Integrate Higgsfield into your applications", Content = "", Category = "APIIntegration", DifficultyLevel = "Intermediate", EstimatedMinutes = 30, SortOrder = 9, IsPublished = true, CreatedAt = SeedDate, Tags = ["api", "integration", "rest"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), Title = "Python SDK Deep Dive", Slug = "python-sdk", Summary = "Use the official Python client library", Content = "", Category = "APIIntegration", DifficultyLevel = "Intermediate", EstimatedMinutes = 25, SortOrder = 10, IsPublished = true, CreatedAt = SeedDate, Tags = ["python", "sdk", "api"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), Title = "Webhook Integration", Slug = "webhooks", Summary = "Receive async notifications for generations", Content = "", Category = "APIIntegration", DifficultyLevel = "Advanced", EstimatedMinutes = 20, SortOrder = 11, IsPublished = true, CreatedAt = SeedDate, Tags = ["webhooks", "api", "async"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), Title = "Motion Control Tutorial", Slug = "motion-control", Summary = "Precise control of character actions and expressions", Content = "", Category = "VideoGeneration", DifficultyLevel = "Advanced", EstimatedMinutes = 25, SortOrder = 12, IsPublished = true, CreatedAt = SeedDate, Tags = ["motion", "kling", "character"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000013"), Title = "Soul 2.0 Fashion Photography", Slug = "soul-2-fashion", Summary = "Ultra-realistic fashion visuals with Soul 2.0", Content = "", Category = "ImageGeneration", DifficultyLevel = "Intermediate", EstimatedMinutes = 20, SortOrder = 13, IsPublished = true, CreatedAt = SeedDate, Tags = ["soul", "fashion", "photography"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000014"), Title = "Mixed Media Presets", Slug = "mixed-media", Summary = "Layer mixed media styles for unique artwork", Content = "", Category = "ImageGeneration", DifficultyLevel = "Intermediate", EstimatedMinutes = 15, SortOrder = 14, IsPublished = true, CreatedAt = SeedDate, Tags = ["mixed-media", "presets", "art"] },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000015"), Title = "Model Comparison Guide", Slug = "model-comparison", Summary = "Choose the right model for your use case", Content = "", Category = "Advanced", DifficultyLevel = "Intermediate", EstimatedMinutes = 20, SortOrder = 15, IsPublished = true, CreatedAt = SeedDate, Tags = ["models", "comparison", "choosing"] },
    ];

    internal static readonly TutorialStep[] Steps =
    [
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), TutorialId = Guid.Parse("10000000-0000-0000-0000-000000000001"), StepNumber = 1, Title = "Create Your Account", Instruction = "Visit higgsfield.ai and sign up for a free account. You can use Google or email signup.", ExpectedResult = "You see the Higgsfield dashboard" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), TutorialId = Guid.Parse("10000000-0000-0000-0000-000000000001"), StepNumber = 2, Title = "Explore the Dashboard", Instruction = "Familiarize yourself with the main navigation: Image, Video, Edit, Character, and Apps sections.", ExpectedResult = "You can navigate between all main sections" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), TutorialId = Guid.Parse("10000000-0000-0000-0000-000000000001"), StepNumber = 3, Title = "Generate Your First Image", Instruction = "Click on 'Image' > select 'Seedream 5.0 Lite' (free) > enter a prompt > toggle 'Unlimited' ON > click Generate", ExpectedResult = "An AI-generated image appears within seconds" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), TutorialId = Guid.Parse("10000000-0000-0000-0000-000000000002"), StepNumber = 1, Title = "Choose Your Model", Instruction = "Navigate to Image generation and select a model. Start with Seedream 5.0 Lite (free) or Nano Banana Pro for 4K quality.", ExpectedResult = "Model selector shows available options" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), TutorialId = Guid.Parse("10000000-0000-0000-0000-000000000002"), StepNumber = 2, Title = "Write an Effective Prompt", Instruction = "Be specific: include subject, style, lighting, camera angle, and quality modifiers. Example: 'A golden retriever puppy playing in autumn leaves, golden hour, professional photography, 85mm lens, bokeh'", ExpectedResult = "High-quality image matching your description" },
    ];

    internal static readonly PromptExample[] PromptExamples =
    [
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Title = "Cinematic Landscape", Prompt = "A breathtaking mountain landscape at golden hour, dramatic clouds, mist in the valleys, shot with an IMAX camera, ultra-wide angle, cinematic color grading", Category = "Landscape", MediaType = "image", ModelId = "higgsfield-ai/soul/standard", Tags = ["landscape", "cinematic", "golden-hour"], IsFeatured = true, Upvotes = 42, CreatedAt = SeedDate },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Title = "Fashion Portrait", Prompt = "Professional fashion photography of a woman in an elegant red dress, studio lighting with soft shadows, Vogue magazine style, shot on Hasselblad, editorial look", Category = "Fashion", MediaType = "image", ModelId = "higgsfield-ai/soul/v2", Tags = ["fashion", "portrait", "studio"], IsFeatured = true, Upvotes = 38, CreatedAt = SeedDate },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), Title = "Product Photography", Prompt = "Minimalist product shot of a luxury perfume bottle on a white marble surface, soft directional lighting, water droplets, commercial photography style, clean background", Category = "Product", MediaType = "image", ModelId = "higgsfield-ai/nano-banana/pro", Tags = ["product", "commercial", "minimalist"], IsFeatured = true, Upvotes = 35, CreatedAt = SeedDate },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), Title = "Slow Motion Walk", Prompt = "A person walking in slow motion through a neon-lit Tokyo alley at night, rain reflections on the ground, cinematic camera following from behind", Category = "Motion", MediaType = "video", ModelId = "higgsfield-ai/dop/standard", Tags = ["video", "cinematic", "neon", "slow-motion"], IsFeatured = true, Upvotes = 56, CreatedAt = SeedDate },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000005"), Title = "Camera Orbit", Prompt = "Smooth 360-degree camera orbit around the subject standing in a sunlit meadow, gentle wind moving hair and grass, shallow depth of field", Category = "Motion", MediaType = "video", ModelId = "kling-video/v3/image-to-video", Tags = ["video", "orbit", "camera-movement"], IsFeatured = false, Upvotes = 28, CreatedAt = SeedDate },
    ];
}

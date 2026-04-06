using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HiggsfieldTutorial.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenerationJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    Prompt = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    HiggsfieldRequestId = table.Column<string>(type: "text", nullable: true),
                    ResultUrl = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    MediaType = table.Column<string>(type: "text", nullable: false),
                    ParametersJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptExamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Prompt = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    MediaType = table.Column<string>(type: "text", nullable: false),
                    ResultImageUrl = table.Column<string>(type: "text", nullable: true),
                    ModelId = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "jsonb", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    Upvotes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptExamples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tutorials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutorials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TutorialResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TutorialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorialResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutorialResources_Tutorials_TutorialId",
                        column: x => x.TutorialId,
                        principalTable: "Tutorials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutorialSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TutorialId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Instruction = table.Column<string>(type: "text", nullable: false),
                    CodeSnippet = table.Column<string>(type: "text", nullable: true),
                    ExpectedResult = table.Column<string>(type: "text", nullable: true),
                    ScreenshotUrl = table.Column<string>(type: "text", nullable: true),
                    Tip = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorialSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutorialSteps_Tutorials_TutorialId",
                        column: x => x.TutorialId,
                        principalTable: "Tutorials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PromptExamples",
                columns: new[] { "Id", "Category", "CreatedAt", "IsFeatured", "MediaType", "ModelId", "Prompt", "ResultImageUrl", "Tags", "Title", "Upvotes" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "Landscape", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "image", "higgsfield-ai/soul/standard", "A breathtaking mountain landscape at golden hour, dramatic clouds, mist in the valleys, shot with an IMAX camera, ultra-wide angle, cinematic color grading", null, "[\"landscape\",\"cinematic\",\"golden-hour\"]", "Cinematic Landscape", 42 },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Fashion", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "image", "higgsfield-ai/soul/v2", "Professional fashion photography of a woman in an elegant red dress, studio lighting with soft shadows, Vogue magazine style, shot on Hasselblad, editorial look", null, "[\"fashion\",\"portrait\",\"studio\"]", "Fashion Portrait", 38 },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "Product", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "image", "higgsfield-ai/nano-banana/pro", "Minimalist product shot of a luxury perfume bottle on a white marble surface, soft directional lighting, water droplets, commercial photography style, clean background", null, "[\"product\",\"commercial\",\"minimalist\"]", "Product Photography", 35 },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "Motion", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "video", "higgsfield-ai/dop/standard", "A person walking in slow motion through a neon-lit Tokyo alley at night, rain reflections on the ground, cinematic camera following from behind", null, "[\"video\",\"cinematic\",\"neon\",\"slow-motion\"]", "Slow Motion Walk", 56 },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "Motion", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "video", "kling-video/v3/image-to-video", "Smooth 360-degree camera orbit around the subject standing in a sunlit meadow, gentle wind moving hair and grass, shallow depth of field", null, "[\"video\",\"orbit\",\"camera-movement\"]", "Camera Orbit", 28 }
                });

            migrationBuilder.InsertData(
                table: "Tutorials",
                columns: new[] { "Id", "Category", "Content", "CoverImageUrl", "CreatedAt", "DifficultyLevel", "EstimatedMinutes", "IsPublished", "Slug", "SortOrder", "Summary", "Tags", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "GettingStarted", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Beginner", 10, true, "getting-started", 1, "Learn the basics of Higgsfield AI platform", "[\"intro\",\"basics\"]", "Getting Started with Higgsfield", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "ImageGeneration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Beginner", 15, true, "image-generation-basics", 2, "Create stunning images using text prompts", "[\"images\",\"text-to-image\"]", "Image Generation Basics", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "VideoGeneration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Beginner", 20, true, "video-generation-images", 3, "Transform static images into dynamic videos", "[\"video\",\"image-to-video\"]", "Video Generation from Images", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "PromptEngineering", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 25, true, "prompt-engineering", 4, "Master the art of writing effective prompts", "[\"prompts\",\"advanced\"]", "Prompt Engineering for AI Art", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "CharacterCreation", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 15, true, "soul-id-character", 5, "Create unique AI characters with Soul ID", "[\"character\",\"soul-id\"]", "Soul ID Character Creation", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "CinemaStudio", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Advanced", 30, true, "cinema-studio", 6, "Director-level control over your AI videos", "[\"cinema\",\"studio\",\"directing\"]", "Cinema Studio Masterclass", null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "VisualEffects", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 20, true, "visual-effects", 7, "Apply 80+ VFX presets to your content", "[\"vfx\",\"effects\",\"transitions\"]", "Visual Effects Library", null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "Apps", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Beginner", 15, true, "higgsfield-apps", 8, "One-click content creation with 60+ apps", "[\"apps\",\"one-click\"]", "Using Higgsfield Apps", null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "APIIntegration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 30, true, "api-quick-start", 9, "Integrate Higgsfield into your applications", "[\"api\",\"integration\",\"rest\"]", "API Quick Start Guide", null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "APIIntegration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 25, true, "python-sdk", 10, "Use the official Python client library", "[\"python\",\"sdk\",\"api\"]", "Python SDK Deep Dive", null },
                    { new Guid("10000000-0000-0000-0000-000000000011"), "APIIntegration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Advanced", 20, true, "webhooks", 11, "Receive async notifications for generations", "[\"webhooks\",\"api\",\"async\"]", "Webhook Integration", null },
                    { new Guid("10000000-0000-0000-0000-000000000012"), "VideoGeneration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Advanced", 25, true, "motion-control", 12, "Precise control of character actions and expressions", "[\"motion\",\"kling\",\"character\"]", "Motion Control Tutorial", null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), "ImageGeneration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 20, true, "soul-2-fashion", 13, "Ultra-realistic fashion visuals with Soul 2.0", "[\"soul\",\"fashion\",\"photography\"]", "Soul 2.0 Fashion Photography", null },
                    { new Guid("10000000-0000-0000-0000-000000000014"), "ImageGeneration", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 15, true, "mixed-media", 14, "Layer mixed media styles for unique artwork", "[\"mixed-media\",\"presets\",\"art\"]", "Mixed Media Presets", null },
                    { new Guid("10000000-0000-0000-0000-000000000015"), "Advanced", "", null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Intermediate", 20, true, "model-comparison", 15, "Choose the right model for your use case", "[\"models\",\"comparison\",\"choosing\"]", "Model Comparison Guide", null }
                });

            migrationBuilder.InsertData(
                table: "TutorialSteps",
                columns: new[] { "Id", "CodeSnippet", "ExpectedResult", "Instruction", "ScreenshotUrl", "StepNumber", "Tip", "Title", "TutorialId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), null, "You see the Higgsfield dashboard", "Visit higgsfield.ai and sign up for a free account. You can use Google or email signup.", null, 1, null, "Create Your Account", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), null, "You can navigate between all main sections", "Familiarize yourself with the main navigation: Image, Video, Edit, Character, and Apps sections.", null, 2, null, "Explore the Dashboard", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), null, "An AI-generated image appears within seconds", "Click on 'Image' > select 'Seedream 5.0 Lite' (free) > enter a prompt > toggle 'Unlimited' ON > click Generate", null, 3, null, "Generate Your First Image", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000004"), null, "Model selector shows available options", "Navigate to Image generation and select a model. Start with Seedream 5.0 Lite (free) or Nano Banana Pro for 4K quality.", null, 1, null, "Choose Your Model", new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000005"), null, "High-quality image matching your description", "Be specific: include subject, style, lighting, camera angle, and quality modifiers. Example: 'A golden retriever puppy playing in autumn leaves, golden hour, professional photography, 85mm lens, bokeh'", null, 2, null, "Write an Effective Prompt", new Guid("10000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenerationJobs_HiggsfieldRequestId",
                table: "GenerationJobs",
                column: "HiggsfieldRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorialResources_TutorialId",
                table: "TutorialResources",
                column: "TutorialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tutorials_Slug",
                table: "Tutorials",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutorialSteps_TutorialId",
                table: "TutorialSteps",
                column: "TutorialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenerationJobs");

            migrationBuilder.DropTable(
                name: "PromptExamples");

            migrationBuilder.DropTable(
                name: "TutorialResources");

            migrationBuilder.DropTable(
                name: "TutorialSteps");

            migrationBuilder.DropTable(
                name: "Tutorials");
        }
    }
}

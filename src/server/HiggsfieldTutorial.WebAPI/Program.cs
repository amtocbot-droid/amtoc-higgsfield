using HiggsfieldTutorial.Application.Interfaces;
using HiggsfieldTutorial.Domain.Interfaces;
using HiggsfieldTutorial.Infrastructure.Persistence;
using HiggsfieldTutorial.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(HiggsfieldTutorial.Application.Features.Tutorials.TutorialHandlers).Assembly));

builder.Services.AddAutoMapper(typeof(HiggsfieldTutorial.Application.Features.Tutorials.TutorialHandlers).Assembly);

builder.Services.AddScoped<ITutorialRepository, TutorialRepository>();
builder.Services.AddScoped<IGenerationJobRepository, GenerationJobRepository>();
builder.Services.AddScoped<IPromptExampleRepository, PromptExampleRepository>();
builder.Services.AddScoped<IHiggsfieldApiClient, HiggsfieldApiClient>();
builder.Services.AddHttpClient<HiggsfieldApiClient>();
builder.Services.AddSingleton<IBrowserAutomationService, BrowserAutomationService>();
builder.Services.AddSingleton<IBrowserAutomationService, BrowserAutomationService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseCors();
app.MapControllers();

app.Run();

using HiggsfieldTutorial.Domain.Entities;
using HiggsfieldTutorial.Domain.Interfaces;
using HiggsfieldTutorial.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HiggsfieldTutorial.Infrastructure.Persistence;

public class TutorialRepository(AppDbContext _db) : ITutorialRepository
{
    public async Task<Tutorial?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Tutorials.Include(t => t.Steps).Include(t => t.Resources).FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Tutorial>> GetAllAsync(CancellationToken ct = default)
        => await _db.Tutorials.Include(t => t.Steps).OrderBy(t => t.SortOrder).ToListAsync(ct);

    public async Task<Tutorial> AddAsync(Tutorial entity, CancellationToken ct = default)
    {
        _db.Tutorials.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Tutorial entity, CancellationToken ct = default)
    {
        _db.Tutorials.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _db.Tutorials.Where(t => t.Id == id).ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyList<Tutorial>> GetByCategoryAsync(string category, CancellationToken ct = default)
        => await _db.Tutorials.Include(t => t.Steps).Where(t => t.Category == category && t.IsPublished).OrderBy(t => t.SortOrder).ToListAsync(ct);

    public async Task<Tutorial?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _db.Tutorials.Include(t => t.Steps).Include(t => t.Resources).FirstOrDefaultAsync(t => t.Slug == slug && t.IsPublished, ct);

    public async Task<IReadOnlyList<Tutorial>> GetPublishedAsync(CancellationToken ct = default)
        => await _db.Tutorials.Include(t => t.Steps).Where(t => t.IsPublished).OrderBy(t => t.SortOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<Tutorial>> SearchAsync(string query, CancellationToken ct = default)
        => await _db.Tutorials.Include(t => t.Steps).Where(t => t.IsPublished && (t.Title.Contains(query) || t.Summary.Contains(query))).OrderBy(t => t.SortOrder).ToListAsync(ct);
}

public class GenerationJobRepository(AppDbContext _db) : IGenerationJobRepository
{
    public async Task<GenerationJob?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.GenerationJobs.FindAsync([id], ct);

    public async Task<IReadOnlyList<GenerationJob>> GetAllAsync(CancellationToken ct = default)
        => await _db.GenerationJobs.OrderByDescending(j => j.CreatedAt).ToListAsync(ct);

    public async Task<GenerationJob> AddAsync(GenerationJob entity, CancellationToken ct = default)
    {
        _db.GenerationJobs.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(GenerationJob entity, CancellationToken ct = default)
    {
        _db.GenerationJobs.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _db.GenerationJobs.Where(j => j.Id == id).ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyList<GenerationJob>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await _db.GenerationJobs.Where(j => j.UserId == userId).OrderByDescending(j => j.CreatedAt).ToListAsync(ct);

    public async Task<GenerationJob?> GetByHiggsfieldRequestIdAsync(string requestId, CancellationToken ct = default)
        => await _db.GenerationJobs.FirstOrDefaultAsync(j => j.HiggsfieldRequestId == requestId, ct);
}

public class PromptExampleRepository(AppDbContext _db) : IPromptExampleRepository
{
    public async Task<PromptExample?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.PromptExamples.FindAsync([id], ct);

    public async Task<IReadOnlyList<PromptExample>> GetAllAsync(CancellationToken ct = default)
        => await _db.PromptExamples.OrderByDescending(p => p.Upvotes).ToListAsync(ct);

    public async Task<PromptExample> AddAsync(PromptExample entity, CancellationToken ct = default)
    {
        _db.PromptExamples.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(PromptExample entity, CancellationToken ct = default)
    {
        _db.PromptExamples.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _db.PromptExamples.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyList<PromptExample>> GetByCategoryAsync(string category, CancellationToken ct = default)
        => await _db.PromptExamples.Where(p => p.Category == category).OrderByDescending(p => p.Upvotes).ToListAsync(ct);

    public async Task<IReadOnlyList<PromptExample>> GetFeaturedAsync(CancellationToken ct = default)
        => await _db.PromptExamples.Where(p => p.IsFeatured).OrderByDescending(p => p.Upvotes).ToListAsync(ct);
}

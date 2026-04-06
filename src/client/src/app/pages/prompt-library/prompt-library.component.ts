import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService, PromptExample } from '../../services/api.service';

@Component({
  selector: 'app-prompt-library',
  standalone: true,
  imports: [],
  template: `
    <div class="library-container">
      <h1>Prompt Library</h1>

      <div class="category-tabs">
        <button (click)="selectCategory(null)"
                [class.active]="!activeCategory()">
          All
        </button>
        @for (cat of categories(); track cat.name) {
          <button (click)="selectCategory(cat.name)"
                  [class.active]="activeCategory() === cat.name">
            {{ cat.name }} ({{ cat.count }})
          </button>
        }
      </div>

      @if (loading()) {
        <p class="loading">Loading prompts...</p>
      } @else {
        <div class="prompt-grid">
          @for (prompt of prompts(); track prompt.id) {
            <div class="prompt-card" (click)="usePrompt(prompt)">
              <div class="prompt-header">
                <h3>{{ prompt.title }}</h3>
                <button class="copy-btn" (click)="copyPrompt($event, prompt.prompt)"
                        [attr.data-copied]="copiedId() === prompt.id">
                  @if (copiedId() === prompt.id) {
                    Copied!
                  } @else {
                    Copy
                  }
                </button>
              </div>

              @if (prompt.resultImageUrl) {
                <img [src]="prompt.resultImageUrl" [alt]="prompt.title" class="prompt-image" />
              }

              <p class="prompt-text">{{ prompt.prompt }}</p>

              <div class="prompt-meta">
                <span class="badge category-badge">{{ prompt.category }}</span>
                <span class="badge type-badge">{{ prompt.mediaType }}</span>
                <span class="badge model-badge">{{ prompt.modelId }}</span>
              </div>

              <div class="prompt-tags">
                @for (tag of prompt.tags; track tag) {
                  <span class="tag">{{ tag }}</span>
                }
              </div>

              <div class="prompt-footer">
                <span class="upvotes">{{ prompt.upvotes }} upvotes</span>
                <span class="use-link">Use in playground &rarr;</span>
              </div>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .library-container { max-width: 1200px; margin: 0 auto; padding: 2rem; }
    .library-container h1 { margin: 0 0 1.5rem; font-size: 1.8rem; }

    .category-tabs {
      display: flex; flex-wrap: wrap; gap: 0.5rem; margin-bottom: 2rem;
    }
    .category-tabs button {
      padding: 0.4rem 0.9rem; border: 1px solid #ddd; border-radius: 20px;
      background: #fff; cursor: pointer; font-size: 0.85rem; color: #333;
      transition: all 0.15s;
    }
    .category-tabs button:hover { border-color: #e94560; color: #e94560; }
    .category-tabs button.active { background: #e94560; color: #fff; border-color: #e94560; }

    .prompt-grid {
      display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 1.5rem;
    }
    .prompt-card {
      border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden;
      background: #fff; cursor: pointer; transition: box-shadow 0.2s, transform 0.2s;
    }
    .prompt-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.1); transform: translateY(-2px); }

    .prompt-header {
      display: flex; justify-content: space-between; align-items: flex-start;
      padding: 1rem 1rem 0;
    }
    .prompt-header h3 { margin: 0; font-size: 1rem; flex: 1; }
    .copy-btn {
      flex-shrink: 0; padding: 0.25rem 0.6rem; border: 1px solid #ddd; border-radius: 4px;
      background: #f8f9fa; cursor: pointer; font-size: 0.75rem; color: #333;
      transition: all 0.15s;
    }
    .copy-btn:hover { border-color: #1565c0; color: #1565c0; }
    .copy-btn[data-copied="true"] { background: #e8f5e9; border-color: #4caf50; color: #2e7d32; }

    .prompt-image { width: 100%; height: 180px; object-fit: cover; margin-top: 0.75rem; }
    .prompt-text {
      padding: 0 1rem; font-size: 0.85rem; color: #555; line-height: 1.5; margin: 0.75rem 0;
      display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;
    }
    .prompt-meta { display: flex; gap: 0.4rem; padding: 0 1rem; flex-wrap: wrap; }
    .badge {
      padding: 0.15rem 0.45rem; border-radius: 4px; font-size: 0.7rem; font-weight: 600;
    }
    .category-badge { background: #e8eaf6; color: #283593; }
    .type-badge { background: #fce4ec; color: #880e4f; }
    .model-badge { background: #e0f2f1; color: #00695c; }

    .prompt-tags { display: flex; flex-wrap: wrap; gap: 0.3rem; padding: 0.5rem 1rem 0; }
    .tag { background: #f5f5f5; padding: 0.1rem 0.4rem; border-radius: 3px; font-size: 0.7rem; color: #777; }

    .prompt-footer {
      display: flex; justify-content: space-between; align-items: center;
      padding: 0.75rem 1rem; border-top: 1px solid #f0f0f0; margin-top: 0.5rem;
    }
    .upvotes { font-size: 0.8rem; color: #888; }
    .use-link { font-size: 0.8rem; color: #e94560; font-weight: 600; }

    .loading { text-align: center; color: #888; padding: 3rem; }
  `],
})
export class PromptLibraryComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);

  readonly loading = signal(true);
  readonly prompts = signal<PromptExample[]>([]);
  readonly categories = signal<{ name: string; count: number }[]>([]);
  readonly activeCategory = signal<string | null>(null);
  readonly copiedId = signal<string | null>(null);

  private copyTimeout: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.api.getPromptCategories().subscribe((cats) => this.categories.set(cats));
    this.loadPrompts();
  }

  selectCategory(category: string | null): void {
    this.activeCategory.set(category);
    this.loadPrompts();
  }

  copyPrompt(event: Event, text: string): void {
    event.stopPropagation();
    navigator.clipboard.writeText(text).then(() => {
      const id = (event.target as HTMLElement).closest('.prompt-card')
        ?.querySelector('.use-link')?.getAttribute('data-id') ?? '';
      this.copiedId.set(id);
      if (this.copyTimeout) clearTimeout(this.copyTimeout);
      this.copyTimeout = setTimeout(() => this.copiedId.set(null), 2000);
    });
  }

  usePrompt(prompt: PromptExample): void {
    this.router.navigate(['/generate'], {
      queryParams: { prompt: prompt.prompt, model: prompt.modelId },
    });
  }

  private loadPrompts(): void {
    this.loading.set(true);
    this.api.getPromptExamples(this.activeCategory() ?? undefined).subscribe({
      next: (data) => { this.prompts.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}

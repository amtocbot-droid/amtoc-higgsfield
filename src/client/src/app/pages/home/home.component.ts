import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, PromptExample } from '../../services/api.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="hero">
      <div class="hero-content">
        <h1>Master Higgsfield AI</h1>
        <p class="subtitle">Learn to create stunning AI-generated images and videos with step-by-step tutorials, prompt engineering guides, and a live playground.</p>
        <a routerLink="/generate" class="cta-button">Try it Live</a>
      </div>
    </section>

    <section class="categories">
      <h2>Explore Tutorials</h2>
      <div class="category-grid">
        @for (cat of categories; track cat.slug) {
          <a [routerLink]="['/tutorials']" [queryParams]="{category: cat.slug}" class="category-card">
            <span class="category-icon">{{ cat.icon }}</span>
            <h3>{{ cat.name }}</h3>
          </a>
        }
      </div>
    </section>

    <section class="featured-prompts">
      <h2>Featured Prompt Examples</h2>
      @if (loading()) {
        <p class="loading">Loading prompts...</p>
      } @else {
        <div class="prompt-grid">
          @for (prompt of featuredPrompts(); track prompt.id) {
            <div class="prompt-card">
              @if (prompt.resultImageUrl) {
                <img [src]="prompt.resultImageUrl" [alt]="prompt.title" class="prompt-image" />
              }
              <div class="prompt-body">
                <h4>{{ prompt.title }}</h4>
                <p class="prompt-text">{{ prompt.prompt }}</p>
                <div class="prompt-meta">
                  <span class="badge">{{ prompt.mediaType }}</span>
                  <span class="badge">{{ prompt.modelId }}</span>
                </div>
                <a [routerLink]="['/generate']" [queryParams]="{prompt: prompt.prompt, model: prompt.modelId}" class="try-link">Try this prompt</a>
              </div>
            </div>
          }
        </div>
      }
    </section>
  `,
  styles: [`
    .hero {
      background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
      color: #fff;
      padding: 4rem 2rem;
      text-align: center;
    }
    .hero h1 { font-size: 3rem; margin-bottom: 1rem; }
    .subtitle { font-size: 1.2rem; max-width: 600px; margin: 0 auto 2rem; opacity: 0.9; }
    .cta-button {
      display: inline-block; background: #e94560; color: #fff; padding: 0.8rem 2rem;
      border-radius: 8px; text-decoration: none; font-weight: 600; font-size: 1.1rem;
      transition: background 0.2s;
    }
    .cta-button:hover { background: #c73650; }

    .categories { padding: 3rem 2rem; max-width: 1200px; margin: 0 auto; }
    .categories h2 { text-align: center; margin-bottom: 2rem; font-size: 2rem; }
    .category-grid {
      display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
      gap: 1rem;
    }
    .category-card {
      display: flex; flex-direction: column; align-items: center; padding: 1.5rem 1rem;
      background: #f8f9fa; border-radius: 12px; text-decoration: none; color: #333;
      transition: transform 0.2s, box-shadow 0.2s; border: 1px solid #e0e0e0;
    }
    .category-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
    .category-icon { font-size: 2rem; margin-bottom: 0.5rem; }
    .category-card h3 { font-size: 0.95rem; margin: 0; text-align: center; }

    .featured-prompts { padding: 3rem 2rem; max-width: 1200px; margin: 0 auto; }
    .featured-prompts h2 { text-align: center; margin-bottom: 2rem; font-size: 2rem; }
    .prompt-grid {
      display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem;
    }
    .prompt-card {
      border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden;
      background: #fff; transition: box-shadow 0.2s;
    }
    .prompt-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.1); }
    .prompt-image { width: 100%; height: 180px; object-fit: cover; }
    .prompt-body { padding: 1rem; }
    .prompt-body h4 { margin: 0 0 0.5rem; }
    .prompt-text {
      font-size: 0.85rem; color: #555; margin: 0 0 0.75rem;
      display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;
    }
    .prompt-meta { display: flex; gap: 0.5rem; margin-bottom: 0.75rem; }
    .badge {
      background: #e3f2fd; color: #1565c0; padding: 0.2rem 0.5rem;
      border-radius: 4px; font-size: 0.75rem;
    }
    .try-link { color: #e94560; text-decoration: none; font-weight: 600; font-size: 0.85rem; }
    .try-link:hover { text-decoration: underline; }
    .loading { text-align: center; color: #888; padding: 2rem; }
  `],
})
export class HomeComponent implements OnInit {
  private api = inject(ApiService);

  readonly loading = signal(true);
  readonly featuredPrompts = signal<PromptExample[]>([]);

  readonly categories = [
    { slug: 'getting-started', name: 'Getting Started', icon: '🚀' },
    { slug: 'image-generation', name: 'Image Generation', icon: '🖼️' },
    { slug: 'video-generation', name: 'Video Generation', icon: '🎬' },
    { slug: 'visual-effects', name: 'Visual Effects', icon: '✨' },
    { slug: 'character-creation', name: 'Character Creation', icon: '🧑‍🎨' },
    { slug: 'cinema-studio', name: 'Cinema Studio', icon: '🎥' },
    { slug: 'apps', name: 'Apps', icon: '📱' },
    { slug: 'api-integration', name: 'API Integration', icon: '🔌' },
    { slug: 'prompt-engineering', name: 'Prompt Engineering', icon: '🧠' },
    { slug: 'advanced', name: 'Advanced', icon: '⚙️' },
  ];

  ngOnInit(): void {
    this.api.getPromptExamples(undefined, true).subscribe({
      next: (prompts) => {
        this.featuredPrompts.set(prompts);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}

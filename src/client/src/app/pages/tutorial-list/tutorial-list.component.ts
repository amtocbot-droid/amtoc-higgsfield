import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ApiService, TutorialSummary } from '../../services/api.service';

@Component({
  selector: 'app-tutorial-list',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="layout">
      <aside class="sidebar">
        <h3>Categories</h3>
        <ul class="category-list">
          <li>
            <button (click)="selectCategory(null)"
                    [class.active]="!selectedCategory()">
              All
            </button>
          </li>
          @for (cat of categories; track cat) {
            <li>
              <button (click)="selectCategory(cat)"
                      [class.active]="selectedCategory() === cat">
                {{ cat }}
              </button>
            </li>
          }
        </ul>
      </aside>

      <main class="main-content">
        <div class="search-bar">
          <input type="text"
                 [ngModel]="searchQuery()"
                 (ngModelChange)="onSearch($event)"
                 placeholder="Search tutorials..."
                 class="search-input" />
        </div>

        @if (loading()) {
          <p class="loading">Loading tutorials...</p>
        } @else if (tutorials().length === 0) {
          <p class="empty">No tutorials found.</p>
        } @else {
          <div class="tutorial-grid">
            @for (tutorial of tutorials(); track tutorial.id) {
              <a [routerLink]="['/tutorials', tutorial.slug]" class="tutorial-card">
                @if (tutorial.coverImageUrl) {
                  <img [src]="tutorial.coverImageUrl" [alt]="tutorial.title" class="card-image" />
                }
                <div class="card-body">
                  <h3>{{ tutorial.title }}</h3>
                  <p class="card-summary">{{ tutorial.summary }}</p>
                  <div class="card-meta">
                    <span class="difficulty" [attr.data-level]="tutorial.difficultyLevel">
                      {{ tutorial.difficultyLevel }}
                    </span>
                    <span class="time">{{ tutorial.estimatedMinutes }} min</span>
                  </div>
                  <div class="card-tags">
                    @for (tag of tutorial.tags; track tag) {
                      <span class="tag">{{ tag }}</span>
                    }
                  </div>
                </div>
              </a>
            }
          </div>
        }
      </main>
    </div>
  `,
  styles: [`
    .layout { display: flex; min-height: calc(100vh - 60px); }
    .sidebar {
      width: 240px; padding: 1.5rem; background: #f8f9fa;
      border-right: 1px solid #e0e0e0; flex-shrink: 0;
    }
    .sidebar h3 { margin: 0 0 1rem; font-size: 1.1rem; }
    .category-list { list-style: none; padding: 0; margin: 0; }
    .category-list li { margin-bottom: 0.25rem; }
    .category-list button {
      width: 100%; text-align: left; padding: 0.5rem 0.75rem; border: none;
      background: none; border-radius: 6px; cursor: pointer; font-size: 0.9rem;
      color: #333; transition: background 0.15s;
    }
    .category-list button:hover { background: #e8eaf6; }
    .category-list button.active { background: #e94560; color: #fff; font-weight: 600; }

    .main-content { flex: 1; padding: 1.5rem 2rem; }
    .search-input {
      width: 100%; max-width: 500px; padding: 0.6rem 1rem; font-size: 1rem;
      border: 1px solid #ccc; border-radius: 8px; margin-bottom: 1.5rem;
    }
    .tutorial-grid {
      display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem;
    }
    .tutorial-card {
      display: block; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden;
      text-decoration: none; color: #333; background: #fff;
      transition: box-shadow 0.2s, transform 0.2s;
    }
    .tutorial-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.1); transform: translateY(-2px); }
    .card-image { width: 100%; height: 160px; object-fit: cover; }
    .card-body { padding: 1rem; }
    .card-body h3 { margin: 0 0 0.5rem; font-size: 1.1rem; }
    .card-summary { color: #555; font-size: 0.85rem; margin: 0 0 0.75rem; line-height: 1.4; }
    .card-meta { display: flex; align-items: center; gap: 0.75rem; margin-bottom: 0.5rem; }
    .difficulty {
      padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 600;
    }
    .difficulty[data-level="Beginner"] { background: #e8f5e9; color: #2e7d32; }
    .difficulty[data-level="Intermediate"] { background: #fff3e0; color: #ef6c00; }
    .difficulty[data-level="Advanced"] { background: #fce4ec; color: #c62828; }
    .time { font-size: 0.8rem; color: #888; }
    .card-tags { display: flex; flex-wrap: wrap; gap: 0.3rem; }
    .tag { background: #f0f0f0; padding: 0.15rem 0.4rem; border-radius: 3px; font-size: 0.7rem; color: #666; }
    .loading, .empty { text-align: center; color: #888; padding: 3rem; }
  `],
})
export class TutorialListComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);

  readonly loading = signal(true);
  readonly tutorials = signal<TutorialSummary[]>([]);
  readonly searchQuery = signal('');
  readonly selectedCategory = signal<string | null>(null);

  readonly categories = [
    'Getting Started',
    'Image Generation',
    'Video Generation',
    'Visual Effects',
    'Character Creation',
    'Cinema Studio',
    'Apps',
    'API Integration',
    'Prompt Engineering',
    'Advanced',
  ];

  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      if (params['category']) {
        this.selectedCategory.set(params['category']);
      }
      this.loadTutorials();
    });
  }

  selectCategory(category: string | null): void {
    this.selectedCategory.set(category);
    this.loadTutorials();
  }

  onSearch(query: string): void {
    this.searchQuery.set(query);
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => this.loadTutorials(), 300);
  }

  private loadTutorials(): void {
    this.loading.set(true);
    this.api.getTutorials(this.selectedCategory() ?? undefined, this.searchQuery() || undefined)
      .subscribe({
        next: (data) => { this.tutorials.set(data); this.loading.set(false); },
        error: () => this.loading.set(false),
      });
  }
}

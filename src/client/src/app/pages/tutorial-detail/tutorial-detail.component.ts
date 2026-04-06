import { Component, inject, signal, input, computed, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, TutorialDetail } from '../../services/api.service';

@Component({
  selector: 'app-tutorial-detail',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="detail-container">
      @if (loading()) {
        <p class="loading">Loading tutorial...</p>
      } @else if (error()) {
        <p class="error">{{ error() }}</p>
      } @else if (tutorial(); as tut) {
        <a routerLink="/tutorials" class="back-link">&larr; Back to Tutorials</a>

        <header class="tutorial-header">
          <h1>{{ tut.title }}</h1>
          <p class="summary">{{ tut.summary }}</p>
          <div class="meta">
            <span class="difficulty" [attr.data-level]="tut.difficultyLevel">
              {{ tut.difficultyLevel }}
            </span>
            <span class="time">{{ tut.estimatedMinutes }} min read</span>
            <span class="category">{{ tut.category }}</span>
          </div>
          <div class="tags">
            @for (tag of tut.tags; track tag) {
              <span class="tag">{{ tag }}</span>
            }
          </div>
        </header>

        <section class="steps">
          <h2>Step-by-Step Walkthrough</h2>
          @for (step of tut.steps; track step.stepNumber) {
            <div class="step">
              <div class="step-number">{{ step.stepNumber }}</div>
              <div class="step-content">
                <h3>{{ step.title }}</h3>
                <p class="instruction">{{ step.instruction }}</p>

                @if (step.codeSnippet) {
                  <div class="code-block">
                    <pre><code>{{ step.codeSnippet }}</code></pre>
                  </div>
                }

                @if (step.expectedResult) {
                  <div class="expected">
                    <strong>Expected result:</strong> {{ step.expectedResult }}
                  </div>
                }

                @if (step.tip) {
                  <div class="tip">
                    <strong>Tip:</strong> {{ step.tip }}
                  </div>
                }
              </div>
            </div>
          }
        </section>

        @if (tut.resources.length > 0) {
          <section class="resources">
            <h2>Resources</h2>
            <ul>
              @for (res of tut.resources; track res.url) {
                <li>
                  <a [href]="res.url" target="_blank" rel="noopener noreferrer">
                    {{ res.label }}
                    <span class="resource-type">({{ res.resourceType }})</span>
                  </a>
                </li>
              }
            </ul>
          </section>
        }
      }
    </div>
  `,
  styles: [`
    .detail-container { max-width: 900px; margin: 0 auto; padding: 2rem; }
    .back-link {
      display: inline-block; margin-bottom: 1.5rem; color: #e94560;
      text-decoration: none; font-weight: 600;
    }
    .back-link:hover { text-decoration: underline; }
    .tutorial-header { margin-bottom: 2.5rem; }
    .tutorial-header h1 { font-size: 2rem; margin: 0 0 0.75rem; }
    .summary { font-size: 1.1rem; color: #555; line-height: 1.5; margin: 0 0 1rem; }
    .meta { display: flex; align-items: center; gap: 1rem; margin-bottom: 0.75rem; }
    .difficulty {
      padding: 0.25rem 0.6rem; border-radius: 4px; font-size: 0.8rem; font-weight: 600;
    }
    .difficulty[data-level="Beginner"] { background: #e8f5e9; color: #2e7d32; }
    .difficulty[data-level="Intermediate"] { background: #fff3e0; color: #ef6c00; }
    .difficulty[data-level="Advanced"] { background: #fce4ec; color: #c62828; }
    .time { font-size: 0.85rem; color: #888; }
    .category { font-size: 0.85rem; color: #1565c0; background: #e3f2fd; padding: 0.2rem 0.5rem; border-radius: 4px; }
    .tags { display: flex; flex-wrap: wrap; gap: 0.3rem; }
    .tag { background: #f0f0f0; padding: 0.15rem 0.5rem; border-radius: 3px; font-size: 0.75rem; color: #666; }

    .steps h2 { margin: 0 0 1.5rem; font-size: 1.5rem; }
    .step {
      display: flex; gap: 1.25rem; margin-bottom: 2rem;
      padding-bottom: 2rem; border-bottom: 1px solid #eee;
    }
    .step:last-child { border-bottom: none; }
    .step-number {
      flex-shrink: 0; width: 40px; height: 40px; border-radius: 50%;
      background: #e94560; color: #fff; display: flex; align-items: center;
      justify-content: center; font-weight: 700; font-size: 1.1rem;
    }
    .step-content { flex: 1; }
    .step-content h3 { margin: 0 0 0.5rem; }
    .instruction { color: #444; line-height: 1.6; margin: 0 0 0.75rem; }
    .code-block {
      background: #1e1e1e; color: #d4d4d4; border-radius: 8px;
      padding: 1rem; margin: 0.75rem 0; overflow-x: auto;
    }
    .code-block pre { margin: 0; font-size: 0.85rem; line-height: 1.5; }
    .code-block code { font-family: 'SF Mono', 'Fira Code', monospace; }
    .expected {
      background: #e8f5e9; border-left: 3px solid #4caf50; padding: 0.6rem 1rem;
      border-radius: 0 6px 6px 0; margin: 0.5rem 0; font-size: 0.9rem;
    }
    .tip {
      background: #fff8e1; border-left: 3px solid #ffc107; padding: 0.6rem 1rem;
      border-radius: 0 6px 6px 0; margin: 0.5rem 0; font-size: 0.9rem;
    }

    .resources { margin-top: 2rem; }
    .resources h2 { margin: 0 0 1rem; font-size: 1.3rem; }
    .resources ul { list-style: none; padding: 0; }
    .resources li { margin-bottom: 0.5rem; }
    .resources a { color: #1565c0; text-decoration: none; }
    .resources a:hover { text-decoration: underline; }
    .resource-type { color: #888; font-size: 0.8rem; }

    .loading, .error { text-align: center; padding: 3rem; color: #888; }
    .error { color: #c62828; }
  `],
})
export class TutorialDetailComponent implements OnInit {
  private api = inject(ApiService);

  readonly slug = input<string>('');

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly tutorial = signal<TutorialDetail | null>(null);

  ngOnInit(): void {
    this.loadTutorial();
  }

  private loadTutorial(): void {
    const slugValue = this.slug();
    if (!slugValue) return;

    this.loading.set(true);
    this.error.set(null);
    this.api.getTutorial(slugValue).subscribe({
      next: (data) => { this.tutorial.set(data); this.loading.set(false); },
      error: () => { this.error.set('Failed to load tutorial.'); this.loading.set(false); },
    });
  }
}

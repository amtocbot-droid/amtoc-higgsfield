import { Component, inject, signal, OnDestroy, OnInit, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService, ModelInfo, GenerationJob } from '../../services/api.service';

@Component({
  selector: 'app-generation',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="playground-container">
      <h1>Generation Playground</h1>

      <div class="layout">
        <div class="form-panel">
          <div class="form-group">
            <label>API Key</label>
            <input type="password" [ngModel]="apiKey()" (ngModelChange)="apiKey.set($event)" placeholder="Your API key" />
          </div>
          <div class="form-group">
            <label>API Secret</label>
            <input type="password" [ngModel]="apiSecret()" (ngModelChange)="apiSecret.set($event)" placeholder="Your API secret" />
          </div>

          <div class="form-group">
            <label>Model</label>
            <select [ngModel]="selectedModelId()" (ngModelChange)="onModelChange($event)">
              <optgroup label="Image Models">
                @for (m of imageModels(); track m.modelId) {
                  <option [value]="m.modelId">{{ m.name }}</option>
                }
              </optgroup>
              <optgroup label="Video Models">
                @for (m of videoModels(); track m.modelId) {
                  <option [value]="m.modelId">{{ m.name }}</option>
                }
              </optgroup>
            </select>
          </div>

          <div class="form-group">
            <label>Prompt</label>
            <textarea [ngModel]="prompt()" (ngModelChange)="prompt.set($event)" rows="4"
                      placeholder="Describe what you want to generate..."></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Image URL (optional)</label>
              <input type="url" [ngModel]="imageUrl()" (ngModelChange)="imageUrl.set($event)"
                     placeholder="https://example.com/image.jpg" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Aspect Ratio</label>
              <select [ngModel]="aspectRatio()" (ngModelChange)="aspectRatio.set($event)">
                <option value="">Default</option>
                <option value="1:1">1:1</option>
                <option value="16:9">16:9</option>
                <option value="9:16">9:16</option>
                <option value="4:3">4:3</option>
                <option value="3:4">3:4</option>
              </select>
            </div>
            <div class="form-group">
              <label>Resolution</label>
              <select [ngModel]="resolution()" (ngModelChange)="resolution.set($event)">
                <option value="">Default</option>
                <option value="512">512px</option>
                <option value="768">768px</option>
                <option value="1024">1024px</option>
                <option value="2048">2048px</option>
              </select>
            </div>
            <div class="form-group">
              <label>Duration (s)</label>
              <input type="number" [ngModel]="duration()" (ngModelChange)="duration.set($event)"
                     min="1" max="30" placeholder="5" />
            </div>
          </div>

          <button class="generate-btn" (click)="generate()"
                  [disabled]="generating() || !apiKey() || !apiSecret() || !selectedModelId() || !prompt()">
            @if (generating()) {
              Generating...
            } @else {
              Generate
            }
          </button>

          @if (errorMessage()) {
            <p class="error">{{ errorMessage() }}</p>
          }
        </div>

        <div class="result-panel">
          <h2>Result</h2>
          @if (status()) {
            <div class="status-display" [attr.data-status]="status()">
              <span class="status-label">Status:</span>
              <span class="status-value">{{ status() }}</span>
              @if (status() === 'queued') {
                <span class="status-icon">⏳</span>
              } @else if (status() === 'in_progress') {
                <span class="status-icon spinner">⟳</span>
              } @else if (status() === 'completed') {
                <span class="status-icon">✓</span>
              } @else if (status() === 'failed') {
                <span class="status-icon">✗</span>
              }
            </div>
          }

          @if (resultUrl() && mediaType() === 'image') {
            <img [src]="resultUrl()" alt="Generated result" class="result-image" />
          } @else if (resultUrl() && mediaType() === 'video') {
            <video [src]="resultUrl()" controls class="result-video"></video>
          } @else if (!status()) {
            <p class="placeholder">Submit a generation to see results here.</p>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .playground-container { max-width: 1200px; margin: 0 auto; padding: 2rem; }
    .playground-container h1 { margin: 0 0 1.5rem; font-size: 1.8rem; }
    .layout { display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; }
    @media (max-width: 768px) { .layout { grid-template-columns: 1fr; } }

    .form-panel, .result-panel {
      background: #fff; border: 1px solid #e0e0e0; border-radius: 12px; padding: 1.5rem;
    }
    .form-group { margin-bottom: 1rem; }
    .form-group label { display: block; font-weight: 600; margin-bottom: 0.3rem; font-size: 0.9rem; }
    .form-group input, .form-group select, .form-group textarea {
      width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #ccc; border-radius: 6px;
      font-size: 0.9rem; box-sizing: border-box;
    }
    .form-group textarea { resize: vertical; font-family: inherit; }
    .form-row { display: flex; gap: 1rem; }
    .form-row .form-group { flex: 1; }

    .generate-btn {
      width: 100%; padding: 0.75rem; background: #e94560; color: #fff; border: none;
      border-radius: 8px; font-size: 1rem; font-weight: 600; cursor: pointer;
      transition: background 0.2s;
    }
    .generate-btn:hover:not(:disabled) { background: #c73650; }
    .generate-btn:disabled { opacity: 0.5; cursor: not-allowed; }

    .error { color: #c62828; margin-top: 1rem; font-size: 0.9rem; }

    .result-panel h2 { margin: 0 0 1rem; font-size: 1.3rem; }
    .status-display {
      display: flex; align-items: center; gap: 0.5rem; padding: 0.6rem 1rem;
      border-radius: 8px; margin-bottom: 1rem; font-size: 0.9rem;
    }
    .status-display[data-status="queued"] { background: #fff3e0; }
    .status-display[data-status="in_progress"] { background: #e3f2fd; }
    .status-display[data-status="completed"] { background: #e8f5e9; }
    .status-display[data-status="failed"] { background: #fce4ec; }
    .status-label { font-weight: 600; }
    .status-value { text-transform: capitalize; }
    .spinner { animation: spin 1s linear infinite; display: inline-block; }
    @keyframes spin { to { transform: rotate(360deg); } }

    .result-image { width: 100%; border-radius: 8px; }
    .result-video { width: 100%; border-radius: 8px; }
    .placeholder { color: #888; text-align: center; padding: 3rem 1rem; }
  `],
})
export class GenerationComponent implements OnInit, OnDestroy {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly models = signal<ModelInfo[]>([]);
  readonly imageModels = signal<ModelInfo[]>([]);
  readonly videoModels = signal<ModelInfo[]>([]);

  readonly apiKey = signal(localStorage.getItem('hf_key') || '');
  readonly apiSecret = signal(localStorage.getItem('hf_secret') || '');
  readonly selectedModelId = signal('');
  readonly prompt = signal('');
  readonly imageUrl = signal('');
  readonly aspectRatio = signal('');
  readonly resolution = signal('');
  readonly duration = signal<number | null>(null);

  readonly generating = signal(false);
  readonly status = signal<string | null>(null);
  readonly resultUrl = signal<string | null>(null);
  readonly mediaType = signal<string>('image');
  readonly errorMessage = signal<string | null>(null);

  private persistKey = effect(() => { localStorage.setItem('hf_key', this.apiKey()); });
  private persistSecret = effect(() => { localStorage.setItem('hf_secret', this.apiSecret()); });
  private pollInterval: ReturnType<typeof setInterval> | null = null;

  ngOnInit(): void {
    this.api.getModels().subscribe((models) => {
      this.models.set(models);
      this.imageModels.set(models.filter((m) => m.mediaType === 'image'));
      this.videoModels.set(models.filter((m) => m.mediaType === 'video'));
    });

    this.route.queryParams.subscribe((params) => {
      if (params['prompt']) this.prompt.set(params['prompt']);
      if (params['model']) this.selectedModelId.set(params['model']);
    });
  }

  ngOnDestroy(): void {
    this.stopPolling();
  }

  onModelChange(modelId: string): void {
    this.selectedModelId.set(modelId);
    const model = this.models().find((m) => m.modelId === modelId);
    if (model) this.mediaType.set(model.mediaType);
  }

  generate(): void {
    if (!this.apiKey() || !this.apiSecret() || !this.selectedModelId() || !this.prompt()) return;

    this.generating.set(true);
    this.status.set('queued');
    this.resultUrl.set(null);
    this.errorMessage.set(null);

    const request: any = {
      modelId: this.selectedModelId(),
      prompt: this.prompt(),
      mediaType: this.mediaType(),
    };
    if (this.imageUrl() && !this.imageUrl()!.includes('example.com')) request.imageUrl = this.imageUrl();
    if (this.aspectRatio()) request.aspectRatio = this.aspectRatio();
    if (this.resolution()) request.resolution = this.resolution();
    if (this.duration()) request.duration = this.duration();

    this.api.submitGeneration(request, this.apiKey(), this.apiSecret()).subscribe({
      next: (job) => {
        this.generating.set(false);
        this.status.set(job.status);
        if (job.status === 'completed' && job.resultUrl) {
          this.resultUrl.set(job.resultUrl);
        } else if (job.status === 'failed' || job.status === 'nsfw') {
          this.api.getGenerationStatus(job.id).subscribe({
            next: (s) => this.errorMessage.set(s.errorMessage || `Generation ${job.status}.`),
            error: () => this.errorMessage.set(`Generation ${job.status}.`),
          });
        } else {
          this.startPolling(job.id);
        }
      },
      error: (err) => {
        this.generating.set(false);
        this.errorMessage.set(err.error?.message || err.message || 'Generation request failed.');
        this.status.set(null);
      },
    });
  }

  private startPolling(jobId: string): void {
    this.stopPolling();
    this.pollInterval = setInterval(() => {
      this.api.getGenerationStatus(jobId).subscribe({
        next: (result) => {
          this.status.set(result.status);
          if (result.status === 'completed' && result.resultUrl) {
            this.resultUrl.set(result.resultUrl);
            this.stopPolling();
          } else if (result.status === 'failed' || result.status === 'nsfw') {
            this.errorMessage.set(result.errorMessage || `Generation ${result.status}.`);
            this.stopPolling();
          }
        },
        error: () => { this.stopPolling(); },
      });
    }, 3000);
  }

  private stopPolling(): void {
    if (this.pollInterval) {
      clearInterval(this.pollInterval);
      this.pollInterval = null;
    }
  }
}

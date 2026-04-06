import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService, AutomationStatus, AutomationJob } from '../../services/api.service';

@Component({
  selector: 'app-automation',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="automation-container">
      <h1>Browser Automation</h1>
      <p class="description">Connect to your Chrome browser (logged in to higgsfield.ai) and automate generation via Playwright.</p>

      <section class="connection-panel">
        <h2>Browser Connection</h2>
        <div class="form-row">
          <div class="form-group">
            <label>CDP URL</label>
            <input type="text" [ngModel]="cdpUrl()" (ngModelChange)="cdpUrl.set($event)"
                   placeholder="http://localhost:9222" />
          </div>
          <div class="button-group">
            @if (!status()?.isConnected) {
              <button class="btn btn-connect" (click)="connect()"
                      [disabled]="connecting()">
                @if (connecting()) {
                  Connecting...
                } @else {
                  Connect to Chrome
                }
              </button>
            } @else {
              <button class="btn btn-disconnect" (click)="disconnect()">
                Disconnect
              </button>
              <span class="status-badge connected">Connected</span>
            }
          </div>
        </div>

        @if (connectionError()) {
          <p class="error">{{ connectionError() }}</p>
        }

        @if (status()?.isConnected) {
          <div class="status-info">
            <span>Job: {{ status()?.currentJobId || 'None' }}</span>
            <span>Status: {{ status()?.currentJobStatus || 'Idle' }}</span>
          </div>
        }
      </section>

      @if (status()?.isConnected) {
        <section class="mode-tabs">
          <button [class.active]="activeMode() === 'image'" (click)="activeMode.set('image')">
            Image
          </button>
          <button [class.active]="activeMode() === 'video'" (click)="activeMode.set('video')">
            Video
          </button>
          <button [class.active]="activeMode() === 'cinema'" (click)="activeMode.set('cinema')">
            Cinema Studio
          </button>
        </section>

        @if (activeMode() === 'image') {
          <section class="generation-panel">
            <h2>Generate Image</h2>
            <div class="form-group">
              <label>Model</label>
              <select [ngModel]="imageModel()" (ngModelChange)="imageModel.set($event)">
                <option value="seedream">Seedream 5.0 Lite (Free)</option>
                <option value="soul">Soul</option>
                <option value="soul-v2">Soul 2.0</option>
                <option value="nano_banana_2">Nano Banana Pro</option>
                <option value="nano_banana">Nano Banana</option>
                <option value="gpt">GPT Image</option>
              </select>
            </div>
            <div class="form-group">
              <label>Prompt</label>
              <textarea [ngModel]="imagePrompt()" (ngModelChange)="imagePrompt.set($event)"
                        rows="4" placeholder="Describe the image you want to generate..."></textarea>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label>Aspect Ratio</label>
                <select [ngModel]="imageAspect()" (ngModelChange)="imageAspect.set($event)">
                  <option value="">Default</option>
                  <option value="1:1">1:1</option>
                  <option value="16:9">16:9</option>
                  <option value="9:16">9:16</option>
                  <option value="4:3">4:3</option>
                  <option value="3:4">3:4</option>
                </select>
              </div>
            </div>
            <button class="btn btn-generate" (click)="generateImage()"
                    [disabled]="generating() || !imagePrompt()">
              @if (generating()) { Generating... } @else { Generate Image }
            </button>
          </section>
        }

        @if (activeMode() === 'video') {
          <section class="generation-panel">
            <h2>Generate Video</h2>
            <div class="form-group">
              <label>Model</label>
              <select [ngModel]="videoModel()" (ngModelChange)="videoModel.set($event)">
                <option value="dop">DoP Preview (Free)</option>
                <option value="kling-v2-1-master">Kling v2.1</option>
                <option value="kling-v2-5-turbo">Kling v2.5 Turbo</option>
                <option value="minimax">MiniMax</option>
                <option value="seedance_pro">Seedance Pro</option>
                <option value="veo-3-preview">Veo 3 Preview</option>
              </select>
            </div>
            <div class="form-group">
              <label>Prompt</label>
              <textarea [ngModel]="videoPrompt()" (ngModelChange)="videoPrompt.set($event)"
                        rows="4" placeholder="Describe the video you want to generate..."></textarea>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label>Aspect Ratio</label>
                <select [ngModel]="videoAspect()" (ngModelChange)="videoAspect.set($event)">
                  <option value="">Default</option>
                  <option value="16:9">16:9</option>
                  <option value="9:16">9:16</option>
                  <option value="1:1">1:1</option>
                </select>
              </div>
              <div class="form-group">
                <label>Duration (s)</label>
                <input type="number" [ngModel]="videoDuration()" (ngModelChange)="videoDuration.set($event)"
                       min="1" max="30" placeholder="5" />
              </div>
            </div>
            <button class="btn btn-generate" (click)="generateVideo()"
                    [disabled]="generating() || !videoPrompt()">
              @if (generating()) { Generating... } @else { Generate Video }
            </button>
          </section>
        }

        @if (activeMode() === 'cinema') {
          <section class="generation-panel">
            <h2>Cinema Studio</h2>
            <p class="hint">Add multiple shots to create a cinematic sequence.</p>

            @for (shot of cinemaShots(); track shot.id; let i = $index) {
              <div class="shot-card">
                <div class="shot-header">
                  <h4>Shot {{ i + 1 }}</h4>
                  @if (cinemaShots().length > 1) {
                    <button class="btn-remove" (click)="removeShot(i)">Remove</button>
                  }
                </div>
                <div class="form-group">
                  <label>Prompt</label>
                  <textarea [ngModel]="shot.prompt" (ngModelChange)="updateShotPrompt(i, $event)"
                            rows="2" placeholder="Describe this shot..."></textarea>
                </div>
                <div class="form-group">
                  <label>Duration (seconds)</label>
                  <input type="number" [ngModel]="shot.durationSeconds" (ngModelChange)="updateShotDuration(i, $event)"
                         min="1" max="30" />
                </div>
              </div>
            }

            <button class="btn btn-add" (click)="addShot()">+ Add Shot</button>

            <button class="btn btn-generate" (click)="generateCinema()"
                    [disabled]="generating() || cinemaShots().length === 0 || cinemaShots().some(s => !s.prompt)">
              @if (generating()) { Generating... } @else { Generate Cinema }
            </button>
          </section>
        }

        @if (jobResult()) {
          <section class="result-panel">
            <h2>Result</h2>
            <div class="result-status" [attr.data-status]="jobResult()?.status">
              <span class="status-label">Status:</span>
              <span class="status-value">{{ jobResult()?.status }}</span>
              <span class="job-id">Job: {{ jobResult()?.jobId }}</span>
            </div>

            @if (jobResult()?.errorMessage) {
              <p class="error">{{ jobResult()?.errorMessage }}</p>
            }

            @if (jobResult()?.resultUrl) {
              <div class="result-media">
                @if (activeMode() === 'video' || activeMode() === 'cinema') {
                  <video [src]="jobResult()?.resultUrl!" controls class="result-video"></video>
                } @else {
                  <img [src]="jobResult()?.resultUrl!" alt="Generated result" class="result-image" />
                }
                <a [href]="jobResult()?.resultUrl!" target="_blank" class="download-link">Open Full Size</a>
              </div>
            }

            @if (jobResult()?.resultUrls && jobResult()!.resultUrls!.length > 0) {
              <div class="result-grid">
                @for (url of jobResult()!.resultUrls!; track url) {
                  <div class="result-item">
                    @if (url.startsWith('error:')) {
                      <p class="error">{{ url }}</p>
                    } @else {
                      <video [src]="url" controls class="result-video"></video>
                    }
                  </div>
                }
              </div>
            }
          </section>
        }
      }
    </div>
  `,
  styles: [`
    .automation-container { max-width: 1000px; margin: 0 auto; padding: 2rem; }
    .automation-container h1 { font-size: 1.8rem; margin: 0 0 0.5rem; }
    .description { color: #666; margin: 0 0 1.5rem; font-size: 0.9rem; }

    .connection-panel, .generation-panel, .result-panel {
      background: #fff; border: 1px solid #e0e0e0; border-radius: 12px;
      padding: 1.5rem; margin-bottom: 1.5rem;
    }
    .connection-panel h2, .generation-panel h2, .result-panel h2 {
      margin: 0 0 1rem; font-size: 1.3rem;
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

    .button-group { display: flex; align-items: center; gap: 1rem; padding-top: 1.5rem; }

    .btn {
      padding: 0.6rem 1.2rem; border: none; border-radius: 8px;
      font-size: 0.9rem; font-weight: 600; cursor: pointer; transition: background 0.2s;
    }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
    .btn-connect { background: #4caf50; color: #fff; }
    .btn-connect:hover:not(:disabled) { background: #388e3c; }
    .btn-disconnect { background: #f44336; color: #fff; }
    .btn-disconnect:hover { background: #d32f2f; }
    .btn-generate { background: #e94560; color: #fff; width: 100%; margin-top: 0.5rem; }
    .btn-generate:hover:not(:disabled) { background: #c73650; }
    .btn-add { background: #1565c0; color: #fff; margin-bottom: 1rem; }
    .btn-add:hover { background: #0d47a1; }
    .btn-remove { background: none; color: #f44336; border: 1px solid #f44336; padding: 0.2rem 0.5rem; font-size: 0.75rem; border-radius: 4px; }

    .status-badge { padding: 0.3rem 0.8rem; border-radius: 20px; font-size: 0.8rem; font-weight: 600; }
    .status-badge.connected { background: #e8f5e9; color: #2e7d32; }

    .status-info {
      display: flex; gap: 1.5rem; margin-top: 0.75rem;
      padding: 0.5rem 1rem; background: #f0f0f0; border-radius: 6px;
      font-size: 0.8rem; color: #555;
    }

    .mode-tabs {
      display: flex; gap: 0.25rem; margin-bottom: 1rem;
    }
    .mode-tabs button {
      flex: 1; padding: 0.6rem; border: 1px solid #ddd; border-radius: 8px 8px 0 00;
      background: #fff; cursor: pointer; font-size: 0.9rem; font-weight: 500;
      transition: all 0.15s;
    }
    .mode-tabs button.active { background: #e94560; color: #fff; border-color: #e94560; }

    .mode-tabs button:hover:not(.active) { border-color: #e94560; color: #e94560; }

    .hint { color: #888; font-size: 0.8rem; margin: 0 0 0.75rem; }

    .shot-card {
      border: 1px solid #e0e0e0; border-radius: 8px; padding: 1rem;
      margin-bottom: 0.75rem; background: #fafafa;
    }
    .shot-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.5rem; }
    .shot-header h4 { margin: 0; font-size: 0.95rem; }

    .result-status {
      display: flex; align-items: center; gap: 0.75rem; padding: 0.6rem 1rem;
      border-radius: 8px; margin-bottom: 1rem;
    }
    .result-status[data-status="completed"] { background: #e8f5e99; }
    .result-status[data-status="failed"] { background: #fce4ec; }
    .result-status[data-status="running"] { background: #e3f2fd; }
    .status-label { font-weight: 600; font-size: 0.9rem; }
    .status-value { text-transform: capitalize; font-size: 0.9rem; }
    .job-id { font-size: 0.8rem; color: #666; }

    .result-media { margin-top: 1rem; text-align: center; }
    .result-image { max-width: 100%; border-radius: 8px; }
    .result-video { width: 100%; border-radius: 8px; }
    .download-link {
      display: inline-block; margin-top: 0.5rem; color: #1565c0;
      text-decoration: none; font-weight: 600; font-size: 0.85rem;
    }
    .download-link:hover { text-decoration: underline; }

    .result-grid {
      display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1rem; margin-top: 1rem;
    }

    .error { color: #c62828; font-size: 0.9rem; margin-top: 0.5rem; }
  `],
})
export class AutomationComponent implements OnInit {
  private api = inject(ApiService);

  readonly cdpUrl = signal('http://host.docker.internal:9222');
  readonly connecting = signal(false);
  readonly connectionError = signal<string | null>(null);
  readonly status = signal<AutomationStatus | null>(null);
  readonly activeMode = signal<'image' | 'video' | 'cinema'>('image');
  readonly generating = signal(false);
  readonly jobResult = signal<AutomationJob | null>(null);

  readonly imageModel = signal('seedream');
  readonly imagePrompt = signal('');
  readonly imageAspect = signal('');
  readonly videoModel = signal('dop');
  readonly videoPrompt = signal('');
  readonly videoAspect = signal('');
  readonly videoDuration = signal<number>(5);
  readonly cinemaShots = signal<{ id: number; prompt: string; durationSeconds: number }[]>([
    { id: 1, prompt: '', durationSeconds: 5 },
  ]);

  private pollInterval: ReturnType<typeof setInterval> | null = null;

  ngOnInit(): void {
    this.refreshStatus();
  }

  connect(): void {
    this.connecting.set(true);
    this.connectionError.set(null);
    this.api.connectBrowser(this.cdpUrl()).subscribe({
      next: (res) => {
        this.connecting.set(false);
        if (res.connected) {
          this.refreshStatus();
        } else {
          this.connectionError.set('Failed to connect. Make sure Chrome is running with --remote-debugging-port=9222');
        }
      },
      error: (err) => {
        this.connecting.set(false);
        this.connectionError.set(err.message || 'Connection failed');
      },
    });
  }

  disconnect(): void {
    this.api.disconnectBrowser().subscribe({
      next: () => this.status.set(null),
      error: () => this.status.set(null),
    });
  }

  generateImage(): void {
    this.generating.set(true);
    this.jobResult.set(null);
    this.api.generateImageAutomation({
      model: this.imageModel(),
      prompt: this.imagePrompt(),
      aspectRatio: this.imageAspect() || undefined,
    }).subscribe({
      next: (job) => {
        this.jobResult.set(job);
        this.generating.set(false);
        this.pollJobStatus(job.jobId);
      },
      error: (err) => {
        this.generating.set(false);
        this.jobResult.set({
          jobId: '', mode: 'image', status: 'failed',
          resultUrl: null, errorMessage: err.message, resultUrls: null,
        });
      },
    });
  }

  generateVideo(): void {
    this.generating.set(true);
    this.jobResult.set(null);
    this.api.generateVideoAutomation({
      model: this.videoModel(),
      prompt: this.videoPrompt(),
      aspectRatio: this.videoAspect() || undefined,
      duration: this.videoDuration(),
    }).subscribe({
      next: (job) => {
        this.jobResult.set(job);
        this.generating.set(false);
        this.pollJobStatus(job.jobId);
      },
      error: (err) => {
        this.generating.set(false);
        this.jobResult.set({
          jobId: '', mode: 'video', status: 'failed',
          resultUrl: null, errorMessage: err.message, resultUrls: null,
        });
      },
    });
  }

  generateCinema(): void {
    this.generating.set(true);
    this.jobResult.set(null);
    this.api.generateCinemaAutomation({
      shots: this.cinemaShots().map(s => ({
        prompt: s.prompt,
        durationSeconds: s.durationSeconds,
      })),
    }).subscribe({
      next: (job) => {
        this.jobResult.set(job);
        this.generating.set(false);
      },
      error: (err) => {
        this.generating.set(false);
        this.jobResult.set({
          jobId: '', mode: 'cinema', status: 'failed',
          resultUrl: null, errorMessage: err.message, resultUrls: null,
        });
      },
    });
  }

  addShot(): void {
    const shots = [...this.cinemaShots()];
    shots.push({ id: Date.now(), prompt: '', durationSeconds: 5 });
    this.cinemaShots.set(shots);
  }

  removeShot(index: number): void {
    const shots = this.cinemaShots().filter((_, i) => i !== index);
    this.cinemaShots.set(shots);
  }

  updateShotPrompt(index: number, prompt: string): void {
    const shots = [...this.cinemaShots()];
    shots[index] = { ...shots[index], prompt };
    this.cinemaShots.set(shots);
  }

  updateShotDuration(index: number, duration: string): void {
    const shots = [...this.cinemaShots()];
    shots[index] = { ...shots[index], durationSeconds: parseInt(duration) || 5 };
    this.cinemaShots.set(shots);
  }

  private refreshStatus(): void {
    this.api.getAutomationStatus().subscribe({
      next: (s) => this.status.set(s),
      error: () => this.status.set(null),
    });
  }

  private pollJobStatus(jobId: string): void {
    if (this.pollInterval) clearInterval(this.pollInterval);
    this.pollInterval = setInterval(() => {
      this.api.getAutomationStatus().subscribe({
        next: (s) => {
          this.status.set(s);
          if (s.currentJobStatus === 'completed' || s.currentJobStatus === 'failed') {
            if (this.pollInterval) clearInterval(this.pollInterval);
          }
        },
      });
    }, 3000);
  }
}

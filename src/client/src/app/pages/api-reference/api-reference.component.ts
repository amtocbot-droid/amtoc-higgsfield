import { Component, inject, signal, OnInit } from '@angular/core';
import { ApiService, ModelInfo } from '../../services/api.service';

@Component({
  selector: 'app-api-reference',
  standalone: true,
  imports: [],
  template: `
    <div class="reference-container">
      <h1>API Reference</h1>

      <nav class="toc">
        <h3>Contents</h3>
        <ul>
          <li><a href="#base-url">Base URL</a></li>
          <li><a href="#authentication">Authentication</a></li>
          <li><a href="#endpoints">Endpoints</a></li>
          <li><a href="#status-values">Status Values</a></li>
          <li><a href="#models">Models</a></li>
          <li><a href="#sdk">Python SDK</a></li>
          <li><a href="#webhooks">Webhooks</a></li>
        </ul>
      </nav>

      <section id="base-url">
        <h2>Base URL</h2>
        <div class="code-block"><pre><code>https://platform.higgsfield.ai</code></pre></div>
      </section>

      <section id="authentication">
        <h2>Authentication</h2>
        <p>All authenticated requests require your API key and secret in the <code>Authorization</code> header:</p>
        <div class="code-block"><pre ngNonBindable><code>Authorization: Key &#123;your_api_key&#125;:&#123;your_api_secret&#125;</code></pre></div>
      </section>

      <section id="endpoints">
        <h2>Endpoints</h2>

        <h3>Submit Generation</h3>
        <div class="endpoint">
          <span class="method post">POST</span>
          <code>/&#123;model_id&#125;</code>
        </div>
        <p>Submit a new generation request for the specified model.</p>
        <div class="code-block"><pre ngNonBindable><code># bash
curl -X POST https://platform.higgsfield.ai/flux-pro-1.1 \
  -H "Authorization: Key YOUR_KEY:YOUR_SECRET" \
  -H "Content-Type: application/json" \
  -d '&#123;
    "prompt": "A sunset over mountains, cinematic",
    "aspect_ratio": "16:9",
    "resolution": "1024"
  &#125;'</code></pre></div>

        <div class="code-block"><pre ngNonBindable><code># python
import higgsfield

client = higgsfield.Client(api_key="YOUR_KEY", api_secret="YOUR_SECRET")

result = client.generate(
    model_id="flux-pro-1.1",
    prompt="A sunset over mountains, cinematic",
    aspect_ratio="16:9",
    resolution="1024"
)
print(result["request_id"])</code></pre></div>

        <h3>Check Status</h3>
        <div class="endpoint">
          <span class="method get">GET</span>
          <code>/requests/&#123;request_id&#125;/status</code>
        </div>
        <div class="code-block"><pre ngNonBindable><code># bash
curl https://platform.higgsfield.ai/requests/abc123/status \
  -H "Authorization: Key YOUR_KEY:YOUR_SECRET"

# python
status = client.get_status("abc123")
print(status["status"], status.get("result_url"))</code></pre></div>

        <h3>Cancel Request</h3>
        <div class="endpoint">
          <span class="method post">POST</span>
          <code>/requests/&#123;request_id&#125;/cancel</code>
        </div>
        <div class="code-block"><pre ngNonBindable><code># bash
curl -X POST https://platform.higgsfield.ai/requests/abc123/cancel \
  -H "Authorization: Key YOUR_KEY:YOUR_SECRET"

# python
client.cancel("abc123")</code></pre></div>
      </section>

      <section id="status-values">
        <h2>Status Values</h2>
        <table>
          <thead>
            <tr><th>Status</th><th>Description</th></tr>
          </thead>
          <tbody>
            <tr><td><code>queued</code></td><td>Request is waiting to be processed</td></tr>
            <tr><td><code>in_progress</code></td><td>Generation is currently running</td></tr>
            <tr><td><code>completed</code></td><td>Generation finished, result available</td></tr>
            <tr><td><code>failed</code></td><td>Generation encountered an error</td></tr>
            <tr><td><code>nsfw</code></td><td>Generation blocked by content filter</td></tr>
          </tbody>
        </table>
      </section>

      <section id="models">
        <h2>Models</h2>
        @if (modelsLoading()) {
          <p class="loading">Loading models...</p>
        } @else {
          <table>
            <thead>
              <tr><th>Model ID</th><th>Name</th><th>Type</th><th>Access</th></tr>
            </thead>
            <tbody>
              @for (model of models(); track model.modelId) {
                <tr>
                  <td><code>{{ model.modelId }}</code></td>
                  <td>{{ model.name }}</td>
                  <td>{{ model.mediaType }}</td>
                  <td>
                    <span class="access" [class.free]="model.isFree" [class.paid]="!model.isFree">
                      {{ model.isFree ? 'Free' : 'Paid' }}
                    </span>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        }
      </section>

      <section id="sdk">
        <h2>Python SDK</h2>
        <p>Install the official Python client:</p>
        <div class="code-block"><pre><code>pip install higgsfield-client</code></pre></div>
        <div class="code-block"><pre ngNonBindable><code>import higgsfield

client = higgsfield.Client(
    api_key="YOUR_KEY",
    api_secret="YOUR_SECRET"
)

# Submit and wait for result
result = client.generateAndWait(
    model_id="flux-pro-1.1",
    prompt="A cinematic shot of a robot in a forest",
    timeout=120
)
print(result["result_url"])</code></pre></div>
      </section>

      <section id="webhooks">
        <h2>Webhooks</h2>
        <p>Pass a <code>hf_webhook</code> query parameter to any generation request to receive a POST callback when the job completes:</p>
        <div class="code-block"><pre ngNonBindable><code>POST /flux-pro-1.1?hf_webhook=https://your-server.com/webhook

# Webhook payload (POST to your URL):
&#123;
  "request_id": "abc123",
  "status": "completed",
  "result_url": "https://cdn.higgsfield.ai/...",
  "model_id": "flux-pro-1.1"
&#125;</code></pre></div>
      </section>
    </div>
  `,
  styles: [`
    .reference-container { max-width: 900px; margin: 0 auto; padding: 2rem; }
    .reference-container h1 { font-size: 2rem; margin: 0 0 1.5rem; }
    .reference-container h2 {
      font-size: 1.4rem; margin: 2rem 0 0.75rem; padding-top: 1rem;
      border-top: 1px solid #eee;
    }
    .reference-container h3 { font-size: 1.1rem; margin: 1.5rem 0 0.5rem; }
    .reference-container p { color: #444; line-height: 1.6; }

    .toc { background: #f8f9fa; border-radius: 8px; padding: 1rem 1.5rem; margin-bottom: 2rem; }
    .toc h3 { margin: 0 0 0.5rem; font-size: 1rem; }
    .toc ul { list-style: none; padding: 0; margin: 0; columns: 2; }
    .toc li { margin-bottom: 0.25rem; }
    .toc a { color: #1565c0; text-decoration: none; font-size: 0.9rem; }
    .toc a:hover { text-decoration: underline; }

    .endpoint { display: flex; align-items: center; gap: 0.75rem; margin: 0.5rem 0; }
    .method {
      padding: 0.2rem 0.6rem; border-radius: 4px; font-size: 0.75rem;
      font-weight: 700; color: #fff; text-transform: uppercase;
    }
    .method.get { background: #43a047; }
    .method.post { background: #1565c0; }
    .endpoint code { font-size: 0.9rem; }

    .code-block {
      background: #1e1e1e; color: #d4d4d4; border-radius: 8px;
      padding: 1rem; margin: 0.75rem 0; overflow-x: auto;
    }
    .code-block pre { margin: 0; font-size: 0.85rem; line-height: 1.5; }
    .code-block code { font-family: 'SF Mono', 'Fira Code', monospace; }

    code {
      background: #f0f0f0; padding: 0.15rem 0.35rem; border-radius: 3px;
      font-family: 'SF Mono', 'Fira Code', monospace; font-size: 0.85rem;
    }

    table { width: 100%; border-collapse: collapse; margin: 1rem 0; }
    th { text-align: left; padding: 0.6rem 0.75rem; background: #f5f5f5; font-size: 0.85rem; }
    td { padding: 0.6rem 0.75rem; border-bottom: 1px solid #eee; font-size: 0.9rem; }
    .access { padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 600; }
    .access.free { background: #e8f5e9; color: #2e7d32; }
    .access.paid { background: #fff3e0; color: #ef6c00; }

    .loading { color: #888; text-align: center; padding: 1.5rem; }
  `],
})
export class ApiReferenceComponent implements OnInit {
  private api = inject(ApiService);

  readonly models = signal<ModelInfo[]>([]);
  readonly modelsLoading = signal(true);

  ngOnInit(): void {
    this.loadModels();
  }

  private loadModels(): void {
    this.api.getModels().subscribe({
      next: (data) => { this.models.set(data); this.modelsLoading.set(false); },
      error: () => this.modelsLoading.set(false),
    });
  }
}

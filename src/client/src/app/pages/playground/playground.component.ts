import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-playground',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="playground-container">
      <h1>Playground</h1>
      <p>The playground is being developed. Visit the <a routerLink="/generate">Generation Playground</a> in the meantime.</p>
    </div>
  `,
  styles: [`
    .playground-container { max-width: 900px; margin: 0 auto; padding: 2rem; text-align: center; }
    .playground-container h1 { font-size: 2rem; margin-bottom: 1rem; }
    .playground-container p { color: #666; font-size: 1.1rem; }
  `],
})
export class PlaygroundComponent {}

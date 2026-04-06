import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },
  { path: 'tutorials', loadComponent: () => import('./pages/tutorial-list/tutorial-list.component').then(m => m.TutorialListComponent) },
  { path: 'tutorials/:slug', loadComponent: () => import('./pages/tutorial-detail/tutorial-detail.component').then(m => m.TutorialDetailComponent) },
  { path: 'generate', loadComponent: () => import('./pages/generation/generation.component').then(m => m.GenerationComponent) },
  { path: 'playground', loadComponent: () => import('./pages/playground/playground.component').then(m => m.PlaygroundComponent) },
  { path: 'prompts', loadComponent: () => import('./pages/prompt-library/prompt-library.component').then(m => m.PromptLibraryComponent) },
  { path: 'api-reference', loadComponent: () => import('./pages/api-reference/api-reference.component').then(m => m.ApiReferenceComponent) },
  { path: 'automate', loadComponent: () => import('./pages/automation/automation.component').then(m => m.AutomationComponent) },
];

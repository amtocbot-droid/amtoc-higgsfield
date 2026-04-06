import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TutorialSummary {
  id: string;
  title: string;
  slug: string;
  summary: string;
  category: string;
  difficultyLevel: string;
  estimatedMinutes: number;
  coverImageUrl: string | null;
  tags: string[];
}

export interface TutorialStep {
  stepNumber: number;
  title: string;
  instruction: string;
  codeSnippet: string | null;
  expectedResult: string | null;
  screenshotUrl: string | null;
  tip: string | null;
}

export interface TutorialResource {
  label: string;
  url: string;
  resourceType: string;
}

export interface TutorialDetail {
  id: string;
  title: string;
  slug: string;
  summary: string;
  category: string;
  difficultyLevel: string;
  estimatedMinutes: number;
  coverImageUrl: string | null;
  tags: string[];
  steps: TutorialStep[];
  resources: TutorialResource[];
}

export interface PromptExample {
  id: string;
  title: string;
  prompt: string;
  category: string;
  mediaType: string;
  resultImageUrl: string | null;
  modelId: string;
  tags: string[];
  isFeatured: boolean;
  upvotes: number;
}

export interface ModelInfo {
  modelId: string;
  name: string;
  mediaType: string;
  description: string;
  isFree: boolean;
  capabilities: string[];
}

export interface GenerationJob {
  id: string;
  modelId: string;
  prompt: string;
  status: string;
  resultUrl: string | null;
  mediaType: string;
  createdAt: string;
  completedAt: string | null;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = '/api';

  constructor(private http: HttpClient) {}

  getTutorials(category?: string, search?: string): Observable<TutorialSummary[]> {
    const params: any = {};
    if (category) params.category = category;
    if (search) params.search = search;
    return this.http.get<TutorialSummary[]>(`${this.baseUrl}/tutorials`, { params });
  }

  getTutorial(slug: string): Observable<TutorialDetail> {
    return this.http.get<TutorialDetail>(`${this.baseUrl}/tutorials/${slug}`);
  }

  getPromptExamples(category?: string, featured?: boolean): Observable<PromptExample[]> {
    const params: any = {};
    if (category) params.category = category;
    if (featured) params.featured = true;
    return this.http.get<PromptExample[]>(`${this.baseUrl}/prompts/examples`, { params });
  }

  getPromptCategories(): Observable<{ name: string; count: number }[]> {
    return this.http.get<{ name: string; count: number }[]>(`${this.baseUrl}/prompts/categories`);
  }

  getModels(): Observable<ModelInfo[]> {
    return this.http.get<ModelInfo[]>(`${this.baseUrl}/prompts/models`);
  }

  submitGeneration(request: {
    modelId: string;
    prompt: string;
    mediaType: string;
    imageUrl?: string;
    aspectRatio?: string;
    resolution?: string;
    duration?: number;
  }, apiKey: string, apiSecret: string): Observable<GenerationJob> {
    return this.http.post<GenerationJob>(`${this.baseUrl}/generation/submit`, request, {
      headers: {
        'X-Higgsfield-Key': apiKey,
        'X-Higgsfield-Secret': apiSecret,
      },
    });
  }

  getGenerationStatus(jobId: string): Observable<{ requestId: string; status: string; resultUrl: string | null; errorMessage: string | null }> {
    return this.http.get<{ requestId: string; status: string; resultUrl: string | null; errorMessage: string | null }>(`${this.baseUrl}/generation/${jobId}/status`);
  }

  cancelGeneration(jobId: string, apiKey: string, apiSecret: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/generation/${jobId}/cancel`, {}, {
      headers: {
        'X-Higgsfield-Key': apiKey,
        'X-Higgsfield-Secret': apiSecret,
      },
    });
  }

  // Automation / Browser control
  connectBrowser(cdpUrl?: string): Observable<{ connected: boolean }> {
    return this.http.post<{ connected: boolean }>(`${this.baseUrl}/automation/connect`, { cdpUrl: cdpUrl || 'http://host.docker.internal:9222' });
  }

  disconnectBrowser(): Observable<any> {
    return this.http.post(`${this.baseUrl}/automation/disconnect`, {});
  }

  getAutomationStatus(): Observable<AutomationStatus> {
    return this.http.get<AutomationStatus>(`${this.baseUrl}/automation/status`);
  }

  generateImageAutomation(request: {
    model: string;
    prompt: string;
    aspectRatio?: string;
    imageUrl?: string;
  }): Observable<AutomationJob> {
    return this.http.post<AutomationJob>(`${this.baseUrl}/automation/generate-image`, request);
  }

  generateVideoAutomation(request: {
    model: string;
    prompt: string;
    aspectRatio?: string;
    duration?: number;
    imageUrl?: string;
  }): Observable<AutomationJob> {
    return this.http.post<AutomationJob>(`${this.baseUrl}/automation/generate-video`, request);
  }

  generateCinemaAutomation(request: {
    shots: { prompt: string; durationSeconds: number }[];
  }): Observable<AutomationJob> {
    return this.http.post<AutomationJob>(`${this.baseUrl}/automation/generate-cinema`, request);
  }
}

export interface AutomationStatus {
  isConnected: boolean;
  currentJobId: string | null;
  currentJobStatus: string | null;
  currentJobMode: string | null;
}

export interface AutomationJob {
  jobId: string;
  mode: string;
  status: string;
  resultUrl: string | null;
  errorMessage: string | null;
  resultUrls: string[] | null;
}

# AGENTS.md — amtoc-higgsfield

## Project Overview

Tutorial PWA for [Higgsfield.ai](https://higgsfield.ai) — an AI media generation platform with 100+ models for images, video, and VFX. The app provides step-by-step tutorials, a prompt library, an API reference, and a live generation playground that calls the Higgsfield API directly.

## Architecture

```
amtoc-higgsfield/
├── src/
│   ├── server/                          # .NET 10 backend
│   │   ├── HiggsfieldTutorial.Domain/   # Entities, enums, interfaces
│   │   ├── HiggsfieldTutorial.Application/ # MediatR handlers, DTOs, interfaces
│   │   ├── HiggsfieldTutorial.Infrastructure/ # EF Core, PostgreSQL, Higgsfield API client
│   │   └── HiggsfieldTutorial.WebAPI/   # Controllers, DI, config
│   └── client/                          # Angular 21 PWA
│       └── src/app/
│           ├── pages/                   # Lazy-loaded route pages
│           ├── services/                # ApiService (HTTP client to backend)
│           └── shared/                  # Shared components
├── docker/
│   ├── Dockerfile.api
│   ├── Dockerfile.client
│   ├── docker-compose.yml               # PostgreSQL + API + Angular
│   └── nginx.conf
└── docs/
```

**Dependency flow**: WebAPI → Application → Domain ← Infrastructure (→ Application)

## Dev Commands

```bash
# Backend
dotnet restore
dotnet build                              # Must succeed before any changes
dotnet run --project src/server/HiggsfieldTutorial.WebAPI

# Frontend
cd src/client
npm install
npm start                                 # Dev server on :4200
npm run build                             # Production build

# Docker (CLI not in PATH — use full path)
export PATH="$HOME/.docker/bin:$PATH"
cd docker && docker compose up --build

# EF Migrations
dotnet ef migrations add <Name> --project src/server/HiggsfieldTutorial.Infrastructure --startup-project src/server/HiggsfieldTutorial.WebAPI
dotnet ef database update --project src/server/HiggsfieldTutorial.Infrastructure --startup-project src/server/HiggsfieldTutorial.WebAPI
```

## Key Technical Details

- **Backend**: .NET 10, Clean Architecture, MediatR CQRS, EF Core 10 + PostgreSQL, Npgsql
- **Frontend**: Angular 21 standalone components, signals, lazy routes, `@angular/material`, `@angular/service-worker` for PWA
- **Higgsfield API**: Async queue pattern — POST to submit, GET to poll status, or use webhooks. Base URL: `https://platform.higgsfield.ai`. Auth: `Key {key}:{secret}`
- **API key flow**: User enters Higgsfield API key/secret in the frontend → passed via custom headers (`X-Higgsfield-Key`, `X-Higgsfield-Secret`) → backend proxies to Higgsfield API
- **Database**: PostgreSQL 17, auto-migrates on startup. Seed data includes 15 tutorials and 5 prompt examples
- **Docker CLI**: Located at `~/.docker/bin/docker` (not in default PATH on this machine)

## API Endpoints

| Endpoint | Method | Description |
|---|---|---|
| `/api/tutorials` | GET | List tutorials (filter by `?category=` or `?search=`) |
| `/api/tutorials/{slug}` | GET | Tutorial detail with steps |
| `/api/tutorials` | POST | Create tutorial |
| `/api/generation/submit` | POST | Submit generation to Higgsfield |
| `/api/generation/{id}/status` | GET | Poll generation status |
| `/api/generation/{id}/cancel` | POST | Cancel queued generation |
| `/api/generation/webhook` | POST | Higgsfield webhook receiver |
| `/api/prompts/examples` | GET | Prompt library (filter by `?category=` or `?featured=true`) |
| `/api/prompts/categories` | GET | Prompt category list |
| `/api/prompts/models` | GET | Available Higgsfield models |
| `/api/automation/connect` | POST | Connect Playwright to Chrome via CDP |
| `/api/automation/disconnect` | POST | Disconnect Playwright from Chrome |
| `/api/automation/status` | GET | Current automation status |
| `/api/automation/generate-image` | POST | Automate image generation via browser |
| `/api/automation/generate-video` | POST | Automate video generation via browser |
| `/api/automation/generate-cinema` | POST | Automate cinema studio multi-shot |

## NuGet Package Notes

- MediatR 12.5.0, FluentValidation 12.0.0
- Npgsql.EntityFrameworkCore.PostgreSQL 10.0.1
- Microsoft.Playwright 1.58.0 (browser automation via CDP)

- **AutoMapper**: Use `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1` (NU1903 warning — non-blocking for dev, upgrade before production)

## Higgsfield Free Models

These models are free/unlimited and should be used for tutorial demos:
- **Images**: Seedream 5.0 Lite (`bytedance/seedream/v5-lite/text-to-image`), Nano Banana
- **Video**: DoP Preview (`higgsfield-ai/dop/preview`), Wan 2.6 (`wan/v2.6/image-to-video`)

## Conventions

- Tutorial slugs are auto-generated from title (lowercase, hyphens)
- Tags stored as PostgreSQL `jsonb` arrays
- Angular pages are lazy-loaded via `loadComponent`
- All Angular components use standalone pattern with signals

# Architecture

## Purpose

The themes repository provides static theme content for MemoAna clients. It is a content distribution application rather than a game server.

## Logical boundaries

```text
MemoAna MAUI
    │
    │ ThemeId + difficulty + BoardSeed
    ▼
MemoAna Backend
    │
    │ game/matchmaking protocol
    ▼
MemoAna MAUI

MemoAna MAUI
    │
    │ HTTPS GET
    ▼
MemoAna Themes (GitHub Pages)
    ├── data/themes.json
    ├── data/<theme-guid>/manifest.json
    ├── data/<theme-guid>/cards.json
    ├── data/errors/*.json
    └── assets/<theme-guid>/*.webp
```

The backend and theme host have intentionally different responsibilities. The backend does not proxy theme images and the theme host does not own game state.

## Application technology

- .NET 10
- Blazor WebAssembly and MudBlazor are retained only as compile-time Razor host/layout infrastructure.
- Static HTML/JSON hosting
- GitHub Pages as the primary deployment target

## Static resolver architecture

The public query routes are implemented as static HTML documents and do not start Blazor WebAssembly.

- `/themes` without query parameters redirects to `/data/themes.json`.
- `/themes?id=<guid>` resolves the catalog and redirects to `/data/<guid>/manifest.json`.
- `/themes?name=<name>` resolves the catalog and redirects to `/data/<guid>/manifest.json`.
- `/cards?id=<guid>` resolves the catalog and redirects to `/data/<guid>/cards.json`.
- `/cards?name=<name>` resolves the catalog and redirects to `/data/<guid>/cards.json`.
- Invalid queries redirect to stable JSON error resources.
- `/version` is a static HTML diagnostic page.
- Razor pages are removed; `MainLayout.razor` remains only for the Razor host to compile cleanly.

## Repository structure

```text
src/themes/
├── Layout/
│   └── MainLayout.razor
└── wwwroot/
    ├── assets/
    ├── cards/index.html
    ├── data/
    ├── js/theme-resolution.js
    ├── version/index.html
    └── index.html
```

## Core decisions

1. Theme content is static and independently cacheable.
2. Theme IDs are stable GUIDs.
3. JSON metadata contains references, not image bytes.
4. Metadata is generated from repository content at build/deployment time.
5. Query resolution is performed by static HTML plus minimal JavaScript; it is not a server-side HTTP endpoint.
6. The no-parameter `/themes` route resolves directly to the generated theme catalog.
7. Actual JSON resources are represented by generated static JSON files.

See the ADRs for rationale and constraints.

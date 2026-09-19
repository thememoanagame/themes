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
    ├── themes.json
    ├── <theme-guid>.manifest.json
    ├── <theme-guid>.cards.json
    └── assets/<theme-guid>/*.webp
```

The backend and theme host have intentionally different responsibilities. The backend does not proxy theme images and the theme host does not own game state.

## Application technology

- .NET 10
- Blazor WebAssembly
- MudBlazor 9
- Static hosting
- GitHub Pages as the primary deployment target

## Repository structure

```text
.github/
├── skills/
│   ├── architecture/
│   ├── blazor-wasm/
│   ├── theme-content/
│   ├── static-api/
│   └── github-pages/
└── workflows/

docs/
└── architecture/
    ├── README.md
    └── decisions/

src/
└── themes/
    ├── Layout/
    ├── Pages/
    └── wwwroot/
        └── assets/
```

## Core decisions

1. Theme content is static and independently cacheable.
2. Theme IDs are stable GUIDs.
3. JSON metadata contains references, not image bytes.
4. Metadata is generated from repository content at build/deployment time.
5. Browser-side query/path resolution is allowed for SPA navigation, but it is not treated as server-side HTTP endpoint execution.
6. Actual JSON resources should be represented by generated static JSON files.

See the ADRs for rationale and constraints.

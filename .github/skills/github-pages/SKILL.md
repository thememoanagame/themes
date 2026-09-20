# GitHub Pages Skill

## Purpose

Use this skill for build, publish, static routing, and deployment changes targeting GitHub Pages.

## Rules

- The published output must be completely static.
- Generate theme metadata before the Pages artifact is published.
- Keep `.json` resources as real static files.
- Do not rely on Blazor SPA fallback for the public theme routes.
- `/themes` is the project-site root and therefore maps to `wwwroot/index.html`.
- `/cards` maps to `wwwroot/cards/index.html`.
- `/version` maps to `wwwroot/version/index.html`.
- Resolver HTML must not load Blazor WebAssembly or MudBlazor.
- Use repository-relative paths so resources work under the configured Pages base path.
- Validate published asset paths and metadata paths after build.
- Do not require a running .NET process after deployment.

# AGENTS.md

## Repository purpose

This repository is the static content and distribution application for MemoAna themes. It is a .NET 10 Blazor WebAssembly application using MudBlazor and is intended to be deployed as a static site, primarily through GitHub Pages.

The repository is a content/distribution boundary, not a game backend. The MemoAna backend communicates canonical game configuration such as `ThemeId`, difficulty, and board seed; it must not transport theme image bytes.

## Current architecture

The solution uses a single Blazor WebAssembly project:

```text
themes.slnx
└── src/
    └── themes/
        ├── App.razor
        ├── Layout/
        ├── Pages/
        ├── Program.cs
        ├── Properties/
        ├── _Imports.razor
        ├── themes.csproj
        └── wwwroot/
            └── assets/
                └── <theme-guid>/
                    └── *.webp
```

The application is intentionally lightweight. Do not introduce backend/server projects, databases, EF Core, authentication, or runtime APIs unless an architecture decision explicitly changes this boundary.

## Architectural rules

1. **Static-first distribution**
   - Theme assets and generated metadata must be consumable as ordinary static files.
   - GitHub Pages is the target hosting model.
   - Runtime server-side execution must not be assumed.

2. **Content is separate from game protocol**
   - The MemoAna backend owns matchmaking and authoritative game state.
   - The themes repository owns theme metadata and image assets.
   - gRPC must carry identifiers and deterministic game configuration, never Base64 image payloads.

3. **Canonical identity**
   - A theme is identified by a stable GUID.
   - Asset directories use the theme GUID.
   - Generated metadata must reference the same GUID.

4. **Generated metadata**
   - Theme indexes/manifests/cards metadata are deployment artifacts derived from the repository's assets.
   - Do not manually duplicate asset inventories when they can be generated.
   - Generated JSON must use stable, deterministic ordering.

5. **Client-side routing**
   - Razor components are UI, not HTTP API endpoints.
   - Query/path aliases may be resolved by a JavaScript module in the Blazor WASM shell.
   - If a consumer needs an actual JSON HTTP response, serve a generated static `.json` file.

6. **No image embedding in JSON**
   - JSON contains IDs, filenames, and URLs.
   - WebP files remain independent static assets.

7. **Blazor/MudBlazor**
   - Use .NET 10 and MudBlazor.
   - Prefer simple components and services over unnecessary abstractions.
   - Keep routing and content-resolution logic separate from presentation.

8. **Documentation**
   - Documentation is written in en-US.
   - Architectural decisions belong under `/docs/architecture/decisions`.
   - Reusable agent guidance belongs under `.github/skills/<skill_name>/SKILL.md`.

## Development conventions

- Nullable reference types remain enabled.
- Implicit usings remain enabled.
- Prefer modern C#/.NET 10 idioms, async APIs where I/O is involved, primary constructors where they improve clarity, and collection expressions where appropriate.
- Avoid speculative frameworks and infrastructure.
- Keep public models explicit and serialization-friendly.
- Do not introduce tests or commits as part of an implementation task unless the task explicitly requests them.
- Do not commit or push changes when acting as a coding agent unless explicitly instructed.

## Theme asset rules

Each theme directory is:

```text
wwwroot/assets/<theme-guid>/
```

Card assets use the established filename convention:

```text
xDD_XTheme_<card-guid>.webp
```

where `DD` identifies the card position/type according to the content-generation convention.

Special assets beginning with `x00_` are selection/featured assets and are not emitted into the ordinary cards collection unless the content contract explicitly says otherwise.

## Before changing architecture

Read:

- `docs/architecture/README.md`
- `docs/architecture/decisions/ADR-0001-static-content-boundary.md`
- `docs/architecture/decisions/ADR-0002-theme-identity-and-layout.md`
- `docs/architecture/decisions/ADR-0003-generated-json-metadata.md`
- `docs/architecture/decisions/ADR-0004-query-path-resolution.md`
- the relevant skill under `.github/skills/`

Do not replace an existing architectural decision silently. Add or supersede an ADR when the decision changes.

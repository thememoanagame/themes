# ADR-0004: Query and Path Resolution

- Status: Accepted
- Date: 2026-09-19

## Context

Consumers may use friendly theme identifiers or query parameters such as:

```text
/themes?id=<guid>
/themes?name=<name>
/cards?id=<guid>
/cards?name=<name>
```

The application is Blazor WebAssembly running on static hosting.

## Decision

Razor resolver pages at `/themes` and `/cards` read `id` and `name`, resolve friendly identifiers against `data/themes.json`, and navigate to `data/<guid>/manifest.json` or `data/<guid>/cards.json`. They render no UI.

The JavaScript layer is client-side routing/resolution only. It is not an HTTP server and cannot transform the initial GitHub Pages HTML response into an arbitrary JSON response.

For machine-consumable JSON, clients should request generated static JSON resources directly.

## Consequences

- Query aliases can provide a convenient browser-facing experience.
- Invalid, missing, or conflicting query parameters navigate to a stable JSON error under `data/errors/`.
- Static hosting remains the source of truth.
- API-like semantics are implemented through static files rather than server execution.
- Path aliases that do not map to an existing static file require SPA fallback handling and should not be treated as ordinary server endpoints.

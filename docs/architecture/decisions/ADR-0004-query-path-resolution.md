# ADR-0004: Query and Path Resolution

- Status: Accepted
- Date: 2026-09-19

## Context

Consumers may use friendly theme identifiers or query parameters such as:

```text
/manifest?id=<guid>
/manifest?id=<name>
/cards?id=<guid>
/cards?id=<name>
```

The application is Blazor WebAssembly running on static hosting.

## Decision

A JavaScript module loaded by `index.html` may inspect `location.pathname` and `URLSearchParams`, resolve friendly identifiers against the generated `data/themes.json` index, and navigate to the canonical GUID resource.

The JavaScript layer is client-side routing/resolution only. It is not an HTTP server and cannot transform the initial GitHub Pages HTML response into an arbitrary JSON response.

For machine-consumable JSON, clients should request generated static JSON resources directly.

## Consequences

- Query aliases can provide a convenient browser-facing experience.
- Static hosting remains the source of truth.
- API-like semantics are implemented through static files rather than server execution.
- Path aliases that do not map to an existing static file require SPA fallback handling and should not be treated as ordinary server endpoints.

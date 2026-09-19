# ADR-0001: Static Content Boundary

- Status: Accepted
- Date: 2026-09-19

## Context

MemoAna needs a separate distribution point for theme images and metadata. The application clients need to download theme content without coupling asset delivery to the game backend.

## Decision

The themes repository is a static-content boundary hosted as a Blazor WebAssembly application and deployed to GitHub Pages.

The repository does not become a game backend and does not own matchmaking, game sessions, player state, or authoritative game rules.

## Consequences

### Positive

- Theme assets can be cached independently from the game backend.
- Large image payloads do not pass through gRPC.
- GitHub Pages is sufficient for distribution.
- Theme releases can evolve independently from backend deployments.

### Constraints

- There is no server-side request pipeline.
- Dynamic HTTP API behavior must not be assumed.
- Metadata that must be served as JSON should be generated as static JSON.

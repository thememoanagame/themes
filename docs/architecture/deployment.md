# Deployment Architecture

## Target

The primary hosting target is GitHub Pages.

The application is a Blazor WebAssembly static site. Deployment must therefore produce static files that GitHub Pages can serve without server-side .NET execution.

## Required deployment stages

1. Restore .NET dependencies.
2. Build/publish the Blazor WebAssembly project.
3. Generate deterministic theme JSON metadata from `wwwroot/assets`.
4. Place generated JSON in the published `wwwroot/data` directory, including `themes.json`, one GUID directory per theme, and static error payloads.
5. Configure the GitHub Pages artifact.
6. Publish the resulting static site.

## Important constraint

Do not implement a deployment step that requires a running ASP.NET Core server after publication.

## SPA fallback

Blazor navigation requires SPA fallback behavior for application routes. Static JSON resources must remain real `.json` files and should not be replaced by the SPA fallback document.

## Caching

Theme image files and generated JSON should use stable URLs. A theme GUID and card GUID should not be reused for different content after publication.

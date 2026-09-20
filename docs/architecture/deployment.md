# Deployment Architecture

## Target

The primary hosting target is GitHub Pages. The published artifact is completely static.

## Required deployment stages

1. Restore .NET dependencies.
2. Generate and validate deterministic theme JSON metadata from `wwwroot/assets`.
3. Publish the .NET project so the static `wwwroot` content is assembled.
4. Inject the commit SHA and deployment timestamp into the static `/version/` diagnostic page.
5. Create `.nojekyll`.
6. Deploy the resulting static files to the `gh-pages` branch.

## Static routing

The repository is deployed as the GitHub Pages project site `/themes/`.

- `wwwroot/index.html` is the public `/themes` resolver.
- `wwwroot/cards/index.html` is the public `/themes/cards` resolver.
- `wwwroot/version/index.html` is the public `/themes/version` diagnostic page.
- Resolver pages contain no Blazor WebAssembly or MudBlazor runtime references.
- `wwwroot/data/*.json` remains real static JSON and is never replaced by an SPA fallback document.

The root resolver is intentionally used for `/themes` because the repository name is also the GitHub Pages project-site base path. A `wwwroot/themes/index.html` file would instead produce `/themes/themes/`.

## Runtime boundary

There is no ASP.NET Core server after publication. Query resolution is browser-side navigation over static files.

## Validation

The generator must fail before publish when theme content violates the content contract, including invalid GUIDs, invalid filenames, missing card indexes, duplicate card IDs, or duplicate theme names.

## Caching

Theme image files and generated JSON use stable URLs. Theme and card GUIDs must not be reused for different content after publication.

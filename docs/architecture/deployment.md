# Deployment Architecture

## Target

The primary hosting target is GitHub Pages. The published artifact is completely static.

## Required deployment stages

1. Restore .NET dependencies.
2. Generate and validate deterministic theme JSON metadata from `wwwroot/assets`.
3. Publish the .NET project so the static `wwwroot` content is assembled.
4. Generate `version.json` from the build-version file produced by the custom MSBuild target.
5. Create `.nojekyll`.
6. Deploy the resulting static files to the `gh-pages` branch.

## Static routing

The repository is currently deployed as the GitHub Pages project site `/themes/`.

- `wwwroot/index.html` is the project-site root resolver.
- `wwwroot/cards/index.html` is the public `/themes/cards` resolver.
- `wwwroot/data/version.json` is the root version resource.
- `wwwroot/data/themes.json` is the theme catalog resource.
- Resolver pages contain no Blazor WebAssembly or MudBlazor runtime references.
- `wwwroot/data/*.json` remains real static JSON and is never replaced by an SPA fallback document.

Because the repository name is `themes`, the GitHub Pages project-site base path is also `/themes/`. A separate `/themes` HTTP endpoint cannot coexist with that project-site root on the same host. The logical `/themes -> /data/themes.json` route applies when the site is hosted at a domain root or another non-conflicting base path.

## Runtime boundary

There is no ASP.NET Core server after publication. Runtime navigation is static-file navigation plus minimal browser-side resolution.

## Version metadata

`wwwroot/data/version.json` is generated during the build. A custom MSBuild target writes the resolved version to `wwwroot/data/build-version.txt` before the generator runs. The generator consumes that transient text file and removes it after producing `version.json`.

Version resolution is:

- GitHub Actions tag build: the tag name.
- Non-tag/local build: the assembly `InformationalVersion`.
- If the version cannot be resolved, the MSBuild target fails instead of creating an empty version file.

The generator also validates that every generated JSON payload is non-empty and writes files atomically, preventing a partially written JSON file from being published.

The version resource contains the version, theme count, deterministic catalog checksum, and theme selection metadata.

## Caching

Theme image files and generated JSON use stable URLs. Theme and card GUIDs must not be reused for different content after publication.

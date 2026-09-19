# GitHub Pages Skill

## Purpose

Use this skill for build, publish, routing, and deployment changes targeting GitHub Pages.

## Rules

- The published output must be completely static.
- Generate theme metadata before the Pages artifact is published.
- Keep `.json` resources outside SPA fallback handling.
- Ensure Blazor's SPA fallback remains compatible with application routes.
- Do not require a running .NET process after deployment.
- Use repository-relative URLs that work under the configured Pages base path.
- Validate published asset paths and metadata paths after build.

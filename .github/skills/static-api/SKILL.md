# Static API Skill

## Purpose

Use this skill when implementing manifest, cards, index, or other machine-consumable resources.

## Rules

- Prefer real static JSON files for HTTP JSON resources.
- Generated resources belong under `wwwroot/data/`.
- Do not implement a Razor component as if it were a server API endpoint.
- Do not depend on server-side middleware or runtime file scanning.
- Keep JSON deterministic and cache-friendly.
- Keep image bytes out of JSON.
- Query/path aliases may be resolved client-side, but canonical static JSON URLs remain the source of truth.
- Preserve correct JSON content types by serving `.json` files directly.

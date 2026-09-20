# Static API Skill

## Purpose

Use this skill when implementing manifest, cards, index, error, or other machine-consumable static resources.

## Rules

- Prefer real static JSON files for HTTP JSON resources.
- Generated resources belong under `wwwroot/data/`.
- Do not implement a Razor component as if it were a server API endpoint.
- Do not depend on server-side middleware or runtime file scanning.
- Keep JSON deterministic and cache-friendly.
- Keep image bytes out of JSON.
- Query aliases are resolved by static HTML plus minimal JavaScript.
- `/themes` without parameters resolves to the generated `data/themes.json` catalog.
- Canonical JSON resources remain the source of truth.
- Preserve correct JSON content types by serving `.json` files directly.

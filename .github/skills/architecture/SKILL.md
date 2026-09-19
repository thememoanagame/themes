# Architecture Skill

## Purpose

Use this skill when changing repository structure, responsibilities, boundaries, or architecture.

## Rules

- Read `AGENTS.md` and the ADRs first.
- Preserve the static-content boundary.
- Keep game state, matchmaking, and authoritative game rules out of this repository.
- Prefer small, explicit abstractions.
- Record architectural changes as ADRs under `docs/architecture/decisions`.
- Do not introduce server-side infrastructure without an explicit architecture decision.

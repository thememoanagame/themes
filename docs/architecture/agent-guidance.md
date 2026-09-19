# Agent Guidance

This document explains how coding agents should work in this repository.

## Required workflow

1. Read `AGENTS.md`.
2. Read the relevant skill under `.github/skills/`.
3. Inspect the current implementation before changing it.
4. Follow the accepted ADRs.
5. Prefer the smallest coherent change.
6. Keep static-hosting constraints explicit.
7. Do not modify MemoAna backend responsibilities from this repository.
8. Do not introduce Base64 image transport.
9. Do not add tests, commits, or pushes unless the task explicitly requests them.

## Documentation language

Use en-US for source comments, XML documentation, README text, skills, and architecture documents.

## Verification

For implementation work, verify:

- project builds with .NET 10;
- generated resource paths match the documented contract;
- JSON is deterministic;
- no generated resource contains image bytes;
- static assets remain addressable after publish;
- SPA fallback does not swallow JSON resources.

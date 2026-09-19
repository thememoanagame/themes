# ADR-0002: Theme Identity and Asset Layout

- Status: Accepted
- Date: 2026-09-19

## Decision

Every theme has a stable GUID identity.

Assets are stored under:

```text
wwwroot/assets/<theme-guid>/
```

The GUID is the canonical identifier used by MemoAna clients and the game backend.

Card files follow the established convention:

```text
xDD_XTheme_<card-guid>.webp
```

Special `x00_` assets are selection/featured assets and are not part of the normal cards list.

## Rationale

Using a stable GUID avoids coupling clients to display names and prevents collisions when names are localized or changed.

The directory layout makes an asset URL derivable from the canonical theme identity and keeps themes independently cacheable.

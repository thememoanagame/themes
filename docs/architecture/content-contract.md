# Theme Content Contract

## Canonical identifiers

### Theme

- Type: GUID
- Must remain stable after publication.
- Used as the asset directory name.

### Card

- Type: GUID
- Must remain stable within a theme.
- Used in card metadata and game configuration.

## Assets

Theme assets live under:

```text
wwwroot/assets/<theme-guid>/
```

Card image names follow:

```text
xDD_XTheme_<card-guid>.webp
```

The content pipeline must preserve the original WebP files and must not embed their bytes into JSON.

## Generated resources

```text
/data/themes.json
/data/<theme-guid>/manifest.json
/data/<theme-guid>/cards.json
/data/errors/*.json
```

All generated JSON should be UTF-8, deterministic, and stable in property meaning.

## Client resolution

A MemoAna client should:

1. Obtain the theme index.
2. Resolve the canonical ThemeId.
3. Fetch the theme manifest/cards metadata.
4. Download only the required WebP assets.
5. Cache metadata and assets according to the client cache policy.

The backend may provide the ThemeId and deterministic board seed, but does not provide theme image bytes.

The theme GUID is part of the resource path and must match `manifest.id`, the corresponding `themes.json` entry, and the asset directory GUID. Resolver failures use stable JSON payloads under `/data/errors/`.

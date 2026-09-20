# Theme Content Contract

## Canonical identifiers

### Theme

- Type: GUID.
- Must remain stable after publication.
- Used as the asset directory name.
- The catalog and manifest must use the same canonical theme GUID.

### Card

- Type: GUID.
- Must remain stable within a theme.
- Used in card metadata and game configuration.

## Assets

Theme assets live under:

```text
wwwroot/assets/<theme-guid>/
```

Card image names follow:

```text
<lowercase-theme-initial><DD>_<ThemeName>_<card-guid>.webp
```

`00` is the selection asset and is excluded from the cards collection. `01` through `15` are the fifteen playable cards.

## Generated resources

```text
/data/version.json
/data/themes.json
/data/<theme-guid>/manifest.json
/data/<theme-guid>/cards.json
/data/errors/*.json
```

The version resource is the root catalog summary and contains the build version, theme count, catalog checksum, and theme names/selection URLs. The theme catalog remains available separately as `themes.json`.

The theme catalog has the following shape:

```json
{
  "themes": [
    {
      "id": "<theme-guid>",
      "name": "<theme-name>",
      "url": "/assets/<theme-guid>/<selection-file>.webp"
    }
  ]
}
```

All generated JSON is UTF-8, deterministic, and stable in property meaning.

## Query resolution

The public static routes are browser-side resolvers:

- `/themes` -> `/data/version.json`
- `/themes?id=<theme-guid>` -> `/data/<theme-guid>/manifest.json`
- `/themes?name=<theme-name>` -> `/data/<theme-guid>/manifest.json`
- `/cards?id=<theme-guid>` -> `/data/<theme-guid>/cards.json`
- `/cards?name=<theme-name>` -> `/data/<theme-guid>/cards.json`

`id` and `name` may be supplied together only when they identify the same theme. Unknown or invalid queries redirect to the corresponding JSON error resource.

The resolvers do not start Blazor WebAssembly. They are static HTML documents with minimal JavaScript.

## Client resolution

A MemoAna client should:

1. Obtain the theme index.
2. Resolve the canonical ThemeId.
3. Fetch the theme manifest/cards metadata.
4. Download only the required WebP assets.
5. Cache metadata and assets according to the client cache policy.

The backend may provide the ThemeId and deterministic board seed, but does not provide theme image bytes.

The theme GUID is part of the resource path and must match `manifest.id`, the corresponding `themes.json` entry, and the asset directory GUID. Resolver failures use stable JSON payloads under `/data/errors/`.

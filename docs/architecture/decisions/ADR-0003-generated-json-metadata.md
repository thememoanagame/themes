# ADR-0003: Generated JSON Metadata

- Status: Accepted
- Date: 2026-09-19

## Decision

Theme metadata is generated from the repository's assets during deployment.

The canonical static structure is:

```text
wwwroot/
├── assets/
│   └── <theme-guid>/*.webp
└── data/
    ├── themes.json
    ├── <theme-guid>.manifest.json
    └── <theme-guid>.cards.json
```

### themes.json

Contains the small global theme index:

```json
{
  "themes": [
    {
      "id": "theme-guid",
      "name": "Theme Name"
    }
  ]
}
```

### Theme manifest

Contains theme-level metadata and references to selection/featured assets.

### Theme cards

Contains card identity, filename, and URL. It does not contain Base64 image data.

Example:

```json
{
  "cards": [
    {
      "id": "card-guid",
      "file": "m01_marvel_card-guid.webp",
      "url": "/assets/theme-guid/m01_marvel_card-guid.webp"
    }
  ]
}
```

## Rationale

Static JSON has correct HTTP semantics on GitHub Pages, is cacheable, and avoids pretending that a Razor component is an HTTP API endpoint.

Generated output also prevents hand-maintained inventories from drifting away from actual files.

## Validation and checksum

Generation fails if a theme directory is not a GUID, if its WebP files do not contain exactly one `00` selection asset and one card for every index `01` through `15`, or if filename prefix, theme name, or GUID rules are violated. The `00` asset has its own valid asset GUID and is emitted as the manifest `url`; the manifest `id` remains the directory GUID. `00` is never emitted in the cards collection.

The manifest checksum is SHA-256 over UTF-8 bytes of the card GUIDs in index order, formatted as lowercase `D`-format GUIDs with one trailing newline between/after entries. It depends only on card IDs.

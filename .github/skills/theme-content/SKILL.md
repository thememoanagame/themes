# Theme Content Skill

## Purpose

Use this skill when adding, validating, renaming, or generating theme assets and metadata.

## Rules

- Theme identity is a stable GUID.
- Store assets under `wwwroot/assets/<theme-guid>/`.
- Preserve WebP assets as independent files.
- Card metadata references IDs, filenames, and URLs; never Base64 image bytes.
- Preserve the established `xDD_XTheme_<card-guid>.webp` convention.
- Treat `x00_` assets as special selection/featured assets unless the content contract says otherwise.
- Generate metadata deterministically.
- Never reuse a published GUID for unrelated content.
- Validate references against files before publishing.

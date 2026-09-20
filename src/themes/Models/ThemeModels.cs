namespace themes.Models;

public sealed record ThemeIndex(IReadOnlyList<ThemeIndexItem> Themes);
public sealed record ThemeIndexItem(string Id, string Name);
public sealed record ThemeManifest(string Id, string Name, string Url, string Checksum, int CardCount, string AssetDirectory, string ValidationStatus);
public sealed record ThemeCards(IReadOnlyList<ThemeCard> Cards);
public sealed record ThemeCard(string Id, string File, string Url);

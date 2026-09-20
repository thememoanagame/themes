using System.Net.Http.Json;
using themes.Models;

namespace themes.Services;

public sealed class ThemeCatalogService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private ThemeIndex? _index;

    public async Task<ThemeIndex> GetIndexAsync() => _index ??= await _http.GetFromJsonAsync<ThemeIndex>("data/themes.json")
        ?? throw new InvalidOperationException("The theme index is empty.");

    public async Task<ThemeManifest?> GetManifestAsync(string? identifier)
    {
        var item = await ResolveAsync(identifier);
        return item is null ? null : await _http.GetFromJsonAsync<ThemeManifest>($"data/{item.Id}.manifest.json");
    }

    private async Task<ThemeIndexItem?> ResolveAsync(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return null;
        var index = await GetIndexAsync();
        return index.Themes.FirstOrDefault(t => string.Equals(t.Id, identifier, StringComparison.OrdinalIgnoreCase))
            ?? index.Themes.FirstOrDefault(t => string.Equals(t.Name, identifier, StringComparison.OrdinalIgnoreCase));
    }
}

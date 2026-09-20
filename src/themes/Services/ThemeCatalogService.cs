using System.Net.Http.Json;
using themes.Models;

namespace themes.Services;

public sealed class ThemeCatalogService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private ThemeIndex? _index;

    public async Task<ThemeIndex> GetIndexAsync() => _index ??= await _http.GetFromJsonAsync<ThemeIndex>("data/themes.json")
        ?? throw new InvalidOperationException("The theme index is empty.");

    public async Task<ThemeResolution> ResolveAsync(string? id, string? name)
    {
        var hasId = !string.IsNullOrWhiteSpace(id);
        var hasName = !string.IsNullOrWhiteSpace(name);
        if (!hasId && !hasName)
            return Error("invalid-query.json");

        var parsedId = Guid.Empty;
        if (hasId && !Guid.TryParse(id, out parsedId))
            return Error("invalid-query.json");

        var index = await GetIndexAsync();
        var byId = hasId ? index.Themes.FirstOrDefault(t => string.Equals(t.Id, parsedId.ToString("D"), StringComparison.OrdinalIgnoreCase)) : null;
        var byName = hasName ? index.Themes.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)) : null;
        if (hasId && byId is null || hasName && byName is null)
            return Error("not-found.json");
        if (byId is not null && byName is not null && !string.Equals(byId.Id, byName.Id, StringComparison.OrdinalIgnoreCase))
            return Error("invalid-query.json");

        var item = byId ?? byName!;
        return new ThemeResolution($"data/{item.Id}/manifest.json", $"data/{item.Id}/cards.json");
    }

    private static ThemeResolution Error(string file) => new($"data/errors/{file}", $"data/errors/{file}");
}

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

var argumentsMap = ParseArguments(args);
var assetsRoot = Path.GetFullPath(argumentsMap.GetValueOrDefault("assets") ?? throw new ArgumentException("Missing --assets."));
var outputRoot = Path.GetFullPath(argumentsMap.GetValueOrDefault("output") ?? throw new ArgumentException("Missing --output."));

if (!Directory.Exists(assetsRoot))
    throw new DirectoryNotFoundException($"Assets directory does not exist: {assetsRoot}");

Directory.CreateDirectory(outputRoot);
foreach (var directory in Directory.EnumerateDirectories(outputRoot))
    if (Guid.TryParseExact(Path.GetFileName(directory), "D", out _))
        Directory.Delete(directory, recursive: true);
WriteErrorResources(outputRoot);
var themes = Directory.EnumerateDirectories(assetsRoot)
    .OrderBy(Path.GetFileName, StringComparer.Ordinal)
    .Select(ReadTheme)
    .ToArray();

if (themes.Length == 0)
    throw new InvalidOperationException("No theme asset directories were found.");

ValidateThemeCatalog(themes);

var jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
WriteJson(
    Path.Combine(outputRoot, "themes.json"),
    new
    {
        themes = themes.Select(theme => new
        {
            id = theme.Id.ToString("D"),
            name = theme.Name,
            url = $"/assets/{theme.Id:D}/{theme.SelectionFile}"
        })
    },
    jsonOptions);

WriteJson(
    Path.Combine(outputRoot, "version.json"),
    new
    {
        themes = themes.Select(theme => new
        {
            id = theme.Id.ToString("D"),
            name = theme.Name,
            url = $"/assets/{theme.Id:D}/{theme.SelectionFile}"
        }),
        themeCount = themes.Length,
        checksum = ThemeCatalogChecksum(themes)
    },
    jsonOptions);

foreach (var theme in themes)
{
    var themeOutput = Path.Combine(outputRoot, theme.Id.ToString("D"));
    Directory.CreateDirectory(themeOutput);
    WriteJson(Path.Combine(themeOutput, "manifest.json"), new
    {
        id = theme.Id.ToString("D"),
        name = theme.Name,
        url = $"/assets/{theme.Id:D}/{theme.SelectionFile}",
        checksum = Checksum(theme.Cards.Select(c => c.Id)),
        cardCount = theme.Cards.Count,
        assetDirectory = $"/assets/{theme.Id:D}/",
        validationStatus = "valid"
    }, jsonOptions);
    WriteJson(Path.Combine(themeOutput, "cards.json"), new
    {
        cards = theme.Cards.Select(c => new { id = c.Id.ToString("D"), file = c.File, url = $"/assets/{theme.Id:D}/{c.File}" })
    }, jsonOptions);
}

foreach (var stale in Directory.EnumerateFiles(outputRoot, "*.manifest.json").Concat(Directory.EnumerateFiles(outputRoot, "*.cards.json")))
    File.Delete(stale);

Console.WriteLine($"Generated metadata for {themes.Length} theme(s) in {outputRoot}.");

static ThemeData ReadTheme(string directory)
{
    var directoryName = Path.GetFileName(directory);
    if (!Guid.TryParseExact(directoryName, "D", out var themeId))
        throw new InvalidOperationException($"Theme '{directoryName}': directory name must be a GUID.");

    var allFiles = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly).ToArray();
    var unexpectedFiles = allFiles.Where(f => !string.Equals(Path.GetExtension(f), ".webp", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (unexpectedFiles.Length > 0)
        throw new InvalidOperationException($"Theme '{directoryName}': unexpected non-WebP asset '{Path.GetFileName(unexpectedFiles[0])}'.");
    var files = allFiles
        .Select(Path.GetFileName)
        .Where(f => f is not null).Cast<string>().OrderBy(f => f, StringComparer.Ordinal).ToArray();
    if (files.Length != 16)
        throw new InvalidOperationException($"Theme '{directoryName}': expected exactly one x00 asset and cards 01..15, found {files.Length} WebP files.");

    var parsed = files.Select(file => ParseAsset(directoryName, file)).ToArray();
    var name = parsed.FirstOrDefault(p => p.Index == 0)?.Name ?? parsed.First().Name;
    var expectedPrefix = char.ToLowerInvariant(name[0]);
    if (parsed.Any(p => p.Name != name))
        throw new InvalidOperationException($"Theme '{name}': all filenames must use the same exact theme name.");
    if (parsed.Any(p => p.Prefix != expectedPrefix))
        throw new InvalidOperationException($"Theme '{name}': filenames must start with lowercase initial '{expectedPrefix}'.");

    var selection = parsed.SingleOrDefault(p => p.Index == 0)
        ?? throw new InvalidOperationException($"Theme '{name}': missing the x00 selection asset.");
    var cards = parsed.Where(p => p.Index is >= 1 and <= 15).OrderBy(p => p.Index).ToArray();
    if (cards.Length != 15 || cards.Select(c => c.Index).Distinct().Count() != 15)
        throw new InvalidOperationException($"Theme '{name}': card indexes must contain each value from 01 through 15 exactly once.");

    if (cards.Select(c => c.Id).Distinct().Count() != cards.Length)
        throw new InvalidOperationException($"Theme '{name}': card GUIDs must be unique.");

    return new ThemeData(themeId, name, selection.File, cards.Select(c => new CardData(c.Id, c.File)).ToArray());
}

static ParsedAsset ParseAsset(string theme, string file)
{
    var match = Regex.Match(file, "^(?<prefix>[a-z])(?<index>\\d{2})_(?<name>.+)_(?<id>[0-9a-fA-F-]+)\\.webp$", RegexOptions.CultureInvariant);
    if (!match.Success || !Guid.TryParseExact(match.Groups["id"].Value, "D", out var id))
        throw new InvalidOperationException($"Theme '{theme}': invalid WebP filename '{file}'. Expected <lowercase initial><DD>_<theme>_<GUID>.webp.");
    var index = int.Parse(match.Groups["index"].Value);
    if (index is > 15)
        throw new InvalidOperationException($"Theme '{theme}', file '{file}': index must be 00 or 01..15.");
    return new ParsedAsset(match.Groups["prefix"].Value[0], index, match.Groups["name"].Value, id, file);
}

static void ValidateThemeCatalog(IReadOnlyList<ThemeData> themes)
{
    var duplicateName = themes
        .GroupBy(theme => theme.Name, StringComparer.OrdinalIgnoreCase)
        .FirstOrDefault(group => group.Count() > 1);

    if (duplicateName is not null)
        throw new InvalidOperationException(
            $"Theme names must be unique case-insensitively. Duplicate name: '{duplicateName.Key}'.");

    var duplicateId = themes
        .GroupBy(theme => theme.Id)
        .FirstOrDefault(group => group.Count() > 1);

    if (duplicateId is not null)
        throw new InvalidOperationException(
            $"Theme IDs must be unique. Duplicate ID: '{duplicateId.Key:D}'.");
}

static string ThemeCatalogChecksum(IEnumerable<ThemeData> themes)
{
    var input = string.Join(
        "\n",
        themes.Select(theme => $"{theme.Id:D}|{theme.Name}|{theme.SelectionFile}")) + "\n";

    return Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(input)))
        .ToLowerInvariant();
}

static string Checksum(IEnumerable<Guid> ids)
{
    // SHA-256 over the UTF-8 bytes of card GUIDs in index order, one lowercase D-format GUID per line.
    var input = string.Join("\n", ids.Select(id => id.ToString("D"))) + "\n";
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input))).ToLowerInvariant();
}

static void WriteErrorResources(string outputRoot)
{
    var errorsRoot = Path.Combine(outputRoot, "errors");
    Directory.CreateDirectory(errorsRoot);
    WriteJson(Path.Combine(errorsRoot, "invalid-query.json"), Error("INVALID_QUERY", "The theme query is invalid.", "query", 400), new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    WriteJson(Path.Combine(errorsRoot, "not-found.json"), Error("THEME_NOT_FOUND", "The requested theme was not found.", "theme", 404), new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    WriteJson(Path.Combine(errorsRoot, "invalid-theme.json"), Error("INVALID_THEME", "The theme resource is invalid.", "theme", 422), new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    WriteJson(Path.Combine(errorsRoot, "invalid-manifest.json"), Error("INVALID_MANIFEST", "The theme manifest is invalid.", "manifest", 422), new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    WriteJson(Path.Combine(errorsRoot, "invalid-cards.json"), Error("INVALID_CARDS", "The theme cards resource is invalid.", "cards", 422), new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}

static object Error(string code, string message, string resource, int status) =>
    new { error = new { code, message, resource, status } };

static void WriteJson(string path, object value, JsonSerializerOptions options) =>
    File.WriteAllText(path, JsonSerializer.Serialize(value, options) + Environment.NewLine, new UTF8Encoding(false));

static Dictionary<string, string> ParseArguments(string[] args)
{
    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var i = 0; i < args.Length; i++)
        if (args[i].StartsWith("--", StringComparison.Ordinal) && i + 1 < args.Length)
            result[args[i][2..]] = args[++i];
    return result;
}

record ThemeData(Guid Id, string Name, string SelectionFile, IReadOnlyList<CardData> Cards);
record CardData(Guid Id, string File);
record ParsedAsset(char Prefix, int Index, string Name, Guid Id, string File);

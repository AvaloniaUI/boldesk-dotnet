using System.Text.Json;

namespace BoldDesk.Cli.Services;

public class CliConfig
{
    public string? Domain { get; set; }
    public string? ApiKey { get; set; }

    public bool IsValid => !string.IsNullOrWhiteSpace(Domain) && !string.IsNullOrWhiteSpace(ApiKey);
}

public class ConfigurationService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static string ConfigDirectory
    {
        get
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, ".bolddesk-cli");
        }
    }

    private static string ConfigPath => Path.Combine(ConfigDirectory, "config.json");

    public async Task<CliConfig?> LoadAsync()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return null;
            var json = await File.ReadAllTextAsync(ConfigPath);
            return JsonSerializer.Deserialize<CliConfig>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveAsync(CliConfig config)
    {
        Directory.CreateDirectory(ConfigDirectory);
        var json = JsonSerializer.Serialize(config, JsonOptions);
        await File.WriteAllTextAsync(ConfigPath, json);
    }

    public async Task<CliConfig?> LoadEffectiveAsync()
    {
        // Env vars take precedence if present
        var envDomain = Environment.GetEnvironmentVariable("BOLDDESK_DOMAIN");
        var envKey = Environment.GetEnvironmentVariable("BOLDDESK_API_KEY");

        CliConfig? cfg = await LoadAsync();
        cfg ??= new CliConfig();

        if (!string.IsNullOrWhiteSpace(envDomain)) cfg.Domain = envDomain;
        if (!string.IsNullOrWhiteSpace(envKey)) cfg.ApiKey = envKey;

        return cfg.IsValid ? cfg : null;
    }
}


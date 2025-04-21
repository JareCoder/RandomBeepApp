using System.Text.Json;

namespace BeepGUIApp.Src;
public static class ConfigManager
{
    private const string DefaultConfigPath = "settings.json";

    public static BeepConfig Load()
    {
        try
        {
            if (!File.Exists(DefaultConfigPath))
            {
                var defaultConfig = new BeepConfig();
                Save(defaultConfig);
                return defaultConfig;
            }

            string json = File.ReadAllText(DefaultConfigPath);
            return JsonSerializer.Deserialize<BeepConfig>(json) ?? new BeepConfig(); // Don't want to return null
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading config. Making a new one: {e.Message}");
            return new BeepConfig(); // Return a default config in case of error
        }
    }

    public static bool Save(BeepConfig config)
    {
        try
        {
            var settings = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, settings);
            File.WriteAllText(DefaultConfigPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save config: {ex.Message}");
            return false;
        }

        return true;
    }
}
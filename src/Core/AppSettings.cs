using System.Text.Json;
using System.Text.Json.Serialization;

using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core
{
    public class AppSettings
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WC3LanGame");

        private static readonly string SettingsFilePath = Path.Combine(
            SettingsDirectory, "settings.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public string HostAddress { get; set; }
        public WarcraftVersion? Version { get; set; }
        public WarcraftType? GameType { get; set; }
        public bool AutoReconnect { get; set; }
        public bool LogExpanded { get; set; }

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return new AppSettings();

                string json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            }
            catch (JsonException)
            {
                return new AppSettings();
            }
            catch (IOException)
            {
                return new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(SettingsDirectory);
                string json = JsonSerializer.Serialize(this, JsonOptions);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (IOException)
            {
                // Settings save is best-effort — don't crash if disk is unavailable
            }
        }
    }
}

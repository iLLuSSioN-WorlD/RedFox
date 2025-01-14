using System;
using System.IO;
using System.Text.Json;

namespace DiscordBot
{
    public class TwitchConfigManager
    {
        private const string ConfigFolder = "Resources";
        private const string ConfigFile = "twitch.json";
        public static readonly string ConfigFilePath = Path.Combine(ConfigFolder, ConfigFile); // Полный путь

        public class TwitchConfig
        {
            public string ClientId { get; set; } = "YOUR_CLIENT_ID_HERE";
            public string ClientSecret { get; set; } = "YOUR_CLIENT_SECRET_HERE";
            public string TokenUrl { get; set; } = "https://id.twitch.tv/oauth2/token";
            public string AccessToken { get; set; } // Новый токен, который будем сохранять
        }

        public TwitchConfig LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"[TwitchConfigManager] Файл конфигурации не найден: {path}. Создаём новый...");
                CreateDefaultConfig(path);
            }

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<TwitchConfig>(json) ?? throw new InvalidDataException("Ошибка чтения файла конфигурации.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TwitchConfigManager] Ошибка при чтении конфигурации: {ex.Message}");
                throw;
            }
        }

        public void SaveConfig(TwitchConfig config, string path)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
                Console.WriteLine($"[TwitchConfigManager] Конфигурация успешно сохранена в {Path.GetFullPath(path)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TwitchConfigManager] Ошибка при сохранении конфигурации: {ex.Message}");
                throw;
            }
        }

        private void CreateDefaultConfig(string path)
        {
            try
            {
                // Создаём папку, если её нет
                var folder = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                // Создаём файл с настройками по умолчанию
                var defaultConfig = new TwitchConfig
                {
                    ClientId = "YOUR_CLIENT_ID_HERE",
                    ClientSecret = "YOUR_CLIENT_SECRET_HERE",
                    TokenUrl = "https://id.twitch.tv/oauth2/token",
                    AccessToken = string.Empty
                };
                var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);

                Console.WriteLine($"[TwitchConfigManager] Конфигурационный файл создан по пути: {Path.GetFullPath(path)}");
                Console.WriteLine($"[TwitchConfigManager] Убедитесь, что вы заполнили ClientId и ClientSecret в файле!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TwitchConfigManager] Ошибка при создании файла конфигурации: {ex.Message}");
                throw;
            }
        }
    }
}

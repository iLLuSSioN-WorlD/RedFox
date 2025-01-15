using System;
using System.IO;
using System.Text.Json;

public class TwitchConfig
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AccessToken { get; set; }
    public DateTime TokenExpiry { get; set; }
}

public class TwitchConfigManager
{
    private const string ConfigFolder = "Resources";
    private const string ConfigFile = "twitch.json";
    public static readonly string ConfigFilePath = Path.Combine(ConfigFolder, ConfigFile);

    private TwitchConfig _config;

    public TwitchConfigManager()
    {
        LoadConfig();
    }

    /// <summary>
    /// Загружает конфигурацию из файла, если он существует, или создает новый файл.
    /// </summary>
    private void LoadConfig()
    {
        try
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            if (!File.Exists(ConfigFilePath))
            {
                _config = new TwitchConfig
                {
                    ClientId = "",
                    ClientSecret = "",
                    AccessToken = "",
                    TokenExpiry = DateTime.UtcNow
                };

                SaveConfig();
                Console.WriteLine($"Файл {ConfigFilePath} был создан. Укажите Client ID и Client Secret в файле перед следующим запуском.");
                return;
            }

            var json = File.ReadAllText(ConfigFilePath);
            _config = JsonSerializer.Deserialize<TwitchConfig>(json) ?? new TwitchConfig();

            // Проверяем, что ClientId и ClientSecret заполнены
            if (string.IsNullOrWhiteSpace(_config.ClientId) || string.IsNullOrWhiteSpace(_config.ClientSecret))
            {
                Console.WriteLine("Client ID или Client Secret не настроены в twitch.json. Функциональность Twitch API будет отключена.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в TwitchConfigManager: {ex.Message}");
            _config = new TwitchConfig(); // Заглушка на случай ошибки
        }
    }



    /// <summary>
    /// Сохраняет текущую конфигурацию в файл.
    /// </summary>
    public void SaveConfig()
    {
        var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFilePath, json);
    }

    public TwitchConfig GetConfig()
    {
        return _config;
    }

    public void UpdateConfig(TwitchConfig newConfig)
    {
        _config = newConfig;
        SaveConfig();
    }

    /// <summary>
    /// Обновляет токен доступа и время его истечения в конфигурации.
    /// </summary>
    /// <param name="accessToken">Новый токен доступа.</param>
    /// <param name="expiry">Время истечения токена.</param>
    public void UpdateAccessToken(string accessToken, DateTime expiry)
    {
        _config.AccessToken = accessToken;
        _config.TokenExpiry = expiry;
        SaveConfig();
    }
}

using System;
using System.IO;
using System.Text.Json;


public class Config
{
    public string Token { get; set; } = "YOUR_BOT_TOKEN_HERE"; // Значение по умолчанию
    public string Prefix { get; set; } = "/"; // Значение по умолчанию

    public string LavalinkHost { get; set; } = "127.0.0.1";
    public ushort LavalinkPort { get; set; } = 2333;
    public string LavalinkPassword { get; set; } = "youshallnotpass";

}


public class ConfigManager
{
    private const string ConfigFolder = "Resources";
    private const string ConfigFile = "config.json";
    public static readonly string ConfigFilePath = Path.Combine(ConfigFolder, ConfigFile);

    public Config Config { get; private set; }

    public ConfigManager()
    {
        if (!File.Exists(ConfigFilePath))
        {
            Console.WriteLine($"[ConfigManager] Файл конфигурации не найден: {ConfigFilePath}. Создаём новый...");
            CreateDefaultConfig();
        }

        var json = File.ReadAllText(ConfigFilePath);
        Config = JsonSerializer.Deserialize<Config>(json) ?? throw new InvalidDataException("Ошибка чтения файла конфигурации.");
    }

    private void CreateDefaultConfig()
    {
        try
        {
            // Создаём папку, если её нет
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            // Создаём файл с настройками по умолчанию
            var defaultConfig = new Config();
            var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);

            Console.WriteLine($"[ConfigManager] Конфигурационный файл создан по пути: {ConfigFilePath}");
            Console.WriteLine($"[ConfigManager] Убедитесь, что вы заполнили правильный токен в файле!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ConfigManager] Ошибка при создании файла конфигурации: {ex.Message}");
            throw;
        }
    }
}

using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class TwitchAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _configPath;
        private readonly TwitchConfigManager _configManager;
        private TwitchConfigManager.TwitchConfig _config; // Используем объект конфигурации

        public TwitchAuthService(HttpClient httpClient, string configPath)
        {
            _httpClient = httpClient;
            _configPath = configPath;
            _configManager = new TwitchConfigManager();

            // Загружаем конфигурацию из файла
            _config = _configManager.LoadConfig(configPath);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_config.AccessToken))
            {
                return _config.AccessToken; // Используем сохранённый токен
            }

            // Попытка получить новый токен
            try
            {
                var token = await RequestNewTokenAsync();
                _config.AccessToken = token; // Сохраняем новый токен в конфигурации
                _configManager.SaveConfig(_config, _configPath); // Сохраняем обновлённую конфигурацию в файл
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TwitchAuthService] Ошибка при получении токена: {ex.Message}");
                throw;
            }
        }

        private async Task<string> RequestNewTokenAsync()
        {
            var response = await _httpClient.PostAsync(
                $"{_config.TokenUrl}?client_id={_config.ClientId}&client_secret={_config.ClientSecret}&grant_type=client_credentials",
                null);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TwitchAuthService] Ошибка ответа Twitch API: {errorDetails}");
                throw new Exception($"Не удалось получить токен: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            return data.GetProperty("access_token").GetString();
        }
    }
}

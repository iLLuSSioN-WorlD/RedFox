using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class TwitchDropsService
    {
        private readonly HttpClient _httpClient;
        private readonly TwitchAuthService _authService;
        private readonly TwitchConfigManager _twitchConfigManager;
        private readonly TwitchConfigManager.TwitchConfig _config; // Получаем конфигурацию

        public TwitchDropsService(HttpClient httpClient, TwitchAuthService authService, TwitchConfigManager twitchConfigManager)
        {
            _httpClient = httpClient;
            _authService = authService;
            _twitchConfigManager = twitchConfigManager;

            // Загружаем конфигурацию из файла
            _config = _twitchConfigManager.LoadConfig(TwitchConfigManager.ConfigFilePath);
        }

        public async Task<string> GetActiveDropsAsync()
        {
            var accessToken = await _authService.GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", _config.ClientId); // Используем ClientId из конфигурации
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync("https://api.twitch.tv/helix/drops/campaigns?status=ACTIVE");

            if (!response.IsSuccessStatusCode)
            {
                return $"Ошибка: {response.StatusCode}";
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}

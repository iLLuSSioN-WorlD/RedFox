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
        private readonly TwitchConfigManager.TwitchConfig _config;

        public TwitchDropsService(HttpClient httpClient, TwitchAuthService authService, TwitchConfigManager.TwitchConfig config)
        {
            _httpClient = httpClient;
            _authService = authService;
            _config = config;
        }

        public async Task<string> GetActiveDropsAsync()
        {
            var accessToken = await _authService.GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", _config.ClientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync("https://api.twitch.tv/helix/drops/campaigns?status=ACTIVE");

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TwitchDropsService] Ошибка: {response.StatusCode}\nДетали: {errorDetails}");
                return $"Ошибка: {response.StatusCode}\n{errorDetails}";
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            if (data.GetProperty("data").GetArrayLength() == 0)
            {
                return "Сейчас нет активных Twitch Drops.";
            }

            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
namespace DiscordBot
{
    public class TwitchAuthService
    {
        private readonly TwitchConfigManager _configManager;

        public TwitchAuthService(TwitchConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task AuthenticateAsync()
        {
            try
            {
                // Выполняем запрос к Twitch API для получения нового токена
                var newAccessToken = await FetchNewAccessTokenAsync();
                var tokenExpiry = DateTime.UtcNow.AddHours(1); // Пример: токен действует 1 час

                // Обновляем токен в конфигурации
                _configManager.UpdateAccessToken(newAccessToken, tokenExpiry);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и выбрасываем исключение дальше для обработки
                Console.WriteLine($"Ошибка при попытке авторизации в Twitch API: {ex.Message}");
                throw;
            }
        }

        private async Task<string> FetchNewAccessTokenAsync()
        {
            var twitchConfig = _configManager.GetConfig();

            if (string.IsNullOrEmpty(twitchConfig.ClientId) || string.IsNullOrEmpty(twitchConfig.ClientSecret))
            {
                throw new InvalidOperationException("Client ID или Client Secret не настроены в twitch.json.");
            }

            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("client_id", twitchConfig.ClientId),
            new KeyValuePair<string, string>("client_secret", twitchConfig.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        })
            };

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Не удалось получить токен Twitch API: {response.StatusCode}, {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            Console.Write(jsonResponse);
            var tokenResponse = JsonSerializer.Deserialize<TwitchTokenResponse>(jsonResponse);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Ответ Twitch API не содержит токен.");
            }

            return tokenResponse.AccessToken;
        }

    }


    /// <summary>
    /// Модель ответа от Twitch API при запросе токена.
    /// </summary>
    public class TwitchTokenResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
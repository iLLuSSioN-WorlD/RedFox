using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class TwitchAuthService
{
    private readonly TwitchConfigManager _configManager;
    private readonly HttpClient _httpClient;

    public TwitchAuthService(TwitchConfigManager configManager)
    {
        _configManager = configManager;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Получение токена через Client Credentials Flow.
    /// </summary>
    public async Task AuthenticateAsync()
    {
        var config = _configManager.GetConfig();
        if (string.IsNullOrEmpty(config.ClientId) || string.IsNullOrEmpty(config.ClientSecret))
        {
            throw new InvalidOperationException("Client ID или Client Secret не настроены в twitch.json.");
        }

        var tokenUrl = "https://id.twitch.tv/oauth2/token";
        var requestBody = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", config.ClientId),
            new KeyValuePair<string, string>("client_secret", config.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await _httpClient.PostAsync(tokenUrl, requestBody);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TwitchTokenResponse>(responseContent);

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            throw new Exception("Ошибка получения токена от Twitch API.");
        }

        // Обновляем токен в конфигурации
        _configManager.UpdateAccessToken(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));
        Console.WriteLine("[TwitchAuthService] Токен успешно обновлен и сохранен.");
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

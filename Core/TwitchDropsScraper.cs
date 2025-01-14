using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class TwitchDropsScraper
    {
        private readonly HttpClient _httpClient;

        public TwitchDropsScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetTwitchDropsAsync()
        {
            var url = "https://www.twitch.tv/drops/campaigns";

            try
            {
                // Загружаем HTML-страницу
                var response = await _httpClient.GetStringAsync(url);

                // Используем HtmlAgilityPack для парсинга HTML
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                // Извлекаем данные о Drops
                var dropsInfo = new StringBuilder();
                var dropsNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, Layout-sc-1xcs6mc-0 pMQwv)]");

                if (dropsNodes == null || dropsNodes.Count == 0)
                {
                    return "Не удалось найти активные Twitch Drops.";
                }

                foreach (var node in dropsNodes)
                {
                    // Извлекаем название и описание
                    var title = node.SelectSingleNode(".//h2[contains(@class, CoreText-sc-1txzju1-0 fBNoPz)]")?.InnerText.Trim();
                    var description = node.SelectSingleNode(".//p[contains(@class, 'YOUR_DESCRIPTION_CLASS')]")?.InnerText.Trim();

                    dropsInfo.AppendLine($"Название: {title}");
                    dropsInfo.AppendLine($"Описание: {description}");
                    dropsInfo.AppendLine(new string('-', 20));
                }

                return dropsInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка при парсинге страницы Drops: {ex.Message}";
            }
        }
    }
}

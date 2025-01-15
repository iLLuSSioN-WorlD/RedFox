using Microsoft.Playwright;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class TwitchDropsScraper
    {
        public async Task<string> GetTwitchDropsAsync()
        {
            try
            {
                // Инициализация Playwright
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });


                // Открываем новую страницу
                var page = await browser.NewPageAsync();
                await page.GotoAsync("https://www.twitch.tv/drops");

                // Ожидаем загрузки элементов
                await page.WaitForSelectorAsync(".ScCoreCard-sc-1rga0ze-0");

                // Извлекаем данные о Drops
                var drops = await page.Locator(".ScCoreCard-sc-1rga0ze-0").AllAsync();
                if (drops.Count == 0)
                {
                    return "Сейчас нет активных Twitch Drops.";
                }

                var dropsInfo = new StringBuilder();
                foreach (var drop in drops)
                {
                    var title = await drop.Locator(".ScCoreCard-sc-1rga0ze-0 h3").InnerTextAsync();
                    var description = await drop.Locator(".ScCoreCard-sc-1rga0ze-0 p").InnerTextAsync();

                    dropsInfo.AppendLine($"Название: {title}");
                    dropsInfo.AppendLine($"Описание: {description}");
                    dropsInfo.AppendLine(new string('-', 20));
                }

                return dropsInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка при парсинге Twitch Drops: {ex.Message}";
            }
        }
    }
}

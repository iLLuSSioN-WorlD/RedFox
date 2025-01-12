using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;

        public InteractionHandler(DiscordSocketClient client)
        {
            _client = client;
            _client.InteractionCreated += HandleInteractionAsync;
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            try
            {
                if (interaction is not SocketSlashCommand command) return;

                switch (command.Data.Name)
                {
                    case "ping":
                        await command.RespondAsync("Pong!");
                        break;

                    case "roll":
                        var random = new Random();
                        var roll = random.Next(1, 101);
                        await command.RespondAsync($"Ваше случайное число: {roll}");
                        break;

                    default:
                        await command.RespondAsync("Неизвестная команда.", ephemeral: true);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке команды: {ex.Message}");
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.RespondAsync("Произошла ошибка при выполнении команды.", ephemeral: true);
                }
            }
        }
    }
}

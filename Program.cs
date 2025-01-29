using Discord.WebSocket;
using Discord;
using DiscordBot;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Commands;
using Victoria;
using RedFox.Core.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var configManager = new ConfigManager(); // Загружаем основную конфигурацию

        var client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.DirectMessages |
                             GatewayIntents.MessageContent
        });


        var serviceProvider = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<CommandHandler>()
            .AddSingleton<ICommand, PingCommand>()
            .AddSingleton<ICommand, RollCommand>()
            .AddSingleton<ICommand, DiceCommand>()
            .AddSingleton<ICommand, RpsCommand>()
            .AddSingleton<ICommand, TestMentionedUserCommand>()            
            .AddSingleton<RandomNumberService>()
            .AddSingleton<DuelService>()
            .AddSingleton<EmojiConverterService>()
            .AddSingleton(configManager.Config)

            .BuildServiceProvider();

        var bot = serviceProvider.GetRequiredService<CommandHandler>();
        client.Log += LogAsync;

        await bot.InitializeAsync();
        await Task.Delay(-1);
    }


    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    public async Task InitializeLavaNodeAsync(IServiceProvider serviceProvider)
    {
        var lavaNode = serviceProvider.GetRequiredService<LavaNode<LavaPlayer, LavaTrack>>();

        if (!lavaNode.IsConnected)
        {
            await lavaNode.ConnectAsync();
            Console.WriteLine("LavaNode успешно подключен!");
        }
    }
}

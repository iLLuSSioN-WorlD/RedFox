using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Extensions
{
    public static class DiscordClientExtensions
    {
        public static async Task<SocketMessage> GetNextMessageAsync(
            this DiscordSocketClient client,
            Func<SocketMessage, bool> predicate,
            TimeSpan timeout)
        {
            var taskCompletionSource = new TaskCompletionSource<SocketMessage>();
            var timeoutTask = Task.Delay(timeout);

            async Task MessageReceived(SocketMessage message)
            {
                try
                {
                    if (predicate(message))
                    {
                        taskCompletionSource.TrySetResult(message);
                    }
                }
                catch
                {
                    // Ignore any exceptions in the predicate
                }
            }

            client.MessageReceived += MessageReceived;

            try
            {
                var completedTask = await Task.WhenAny(taskCompletionSource.Task, timeoutTask);
                if (completedTask == timeoutTask)
                {
                    throw new TimeoutException();
                }

                return await taskCompletionSource.Task;
            }
            finally
            {
                client.MessageReceived -= MessageReceived;
            }
        }
    }
}

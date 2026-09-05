using Discord;
using Serilog.Core;
using Serilog.Events;

namespace OlliBot.Bot;
internal class DiscordAlertSink(IDiscordClient discordClient, IConfiguration configuration) : ILogEventSink
{
    public async void Emit(LogEvent logEvent)
    {
        try
        {
            var userId = configuration.GetValue<ulong>("OwnerID");

            IUser? user = await discordClient.GetUserAsync(userId);

            if (user is null)
                return;

            string message = $"[{logEvent.Level}] {logEvent.RenderMessage()}";

            if (logEvent.Exception is not null)
            {
                message += $"\nException: {logEvent.Exception}";
            }

            await user.SendMessageAsync(message);
        }
        catch
        {

        }
    }
}

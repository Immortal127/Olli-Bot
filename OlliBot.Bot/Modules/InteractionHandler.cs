using Discord.Interactions;
using Discord.WebSocket;
using System.Text;

namespace OlliBot.Bot.Modules;

public class InteractionHandler(
    InteractionService interactionService,
    ILogger<BotHostedService> logger,
    DiscordSocketClient client,
    IServiceProvider serviceProvider)
{
    public async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            var context = new SocketInteractionContext(client, arg);
            await interactionService.ExecuteCommandAsync(context, serviceProvider);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error handling interaction.");
        }
    }

    public Task OnSlashInvoked(SocketInteraction interaction)
    {
        var command = interaction as SocketSlashCommand;

        var logMessage = new StringBuilder();

        if (command is null)
        {
            return Task.CompletedTask;
        }

        logMessage.Append($"Command invoked: {command.CommandName} ");

        if (command.Data.Options.Count != 0)
        {
            logMessage.Append("(");
            foreach (SocketSlashCommandDataOption? option in command.Data.Options.Where(option => option != null))
            {
                logMessage.Append($"{option.Name}:{option.Value}, ");
            }
            if (logMessage[logMessage.Length - 2] == ',') // Removing the trailing comma and space
            {
                logMessage.Length -= 2;
            }
            logMessage.Append(") ");
        }
        logMessage.Append($"by {command.User.Username}, {command.User.Id}");
        logger.LogInformation("{Message}", logMessage.ToString());
        return Task.CompletedTask;
    }
}

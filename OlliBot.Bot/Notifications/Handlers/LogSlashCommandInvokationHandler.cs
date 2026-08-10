using Discord.WebSocket;
using MediatR;
using System.Text;

namespace OlliBot.Bot.Notifications.Handlers;
internal class LogSlashCommandInvokationHandler(ILogger<LogSlashCommandInvokationHandler> logger) : INotificationHandler<InteractionCreatedNotification>
{
    public Task Handle(InteractionCreatedNotification notification, CancellationToken cancellationToken)
    {
        var command = notification.Interaction as SocketSlashCommand;

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

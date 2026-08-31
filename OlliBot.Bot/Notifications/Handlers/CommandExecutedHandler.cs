using Discord;
using Discord.Interactions;
using MediatR;

namespace OlliBot.Bot.Notifications.Handlers;
internal class CommandExecutedHandler(ILogger<CommandExecutedHandler> logger) : INotificationHandler<CommandExecutedNotification>
{
    public async Task Handle(
        CommandExecutedNotification notification,
        CancellationToken cancellationToken)
    {
        IResult result = notification.Result;
        IInteractionContext ctx = notification.InteractionContext;
        ICommandInfo commandInfo = notification.CommandInfo;

        string commandType = commandInfo switch
        {
            SlashCommandInfo => "Slash",
            UserCommandInfo => "User",
            MessageCommandInfo => "Message",
            ModalCommandInfo => "Modal",
            ComponentCommandInfo => "Component",
            _ => "Unknown"
        };

        if (result.IsSuccess)
        {
            logger.LogInformation("{Type} Command '{Command}' completed without InteractionService errors", commandType, commandInfo.Name);
            return;
        }

        if (result.Error == InteractionCommandError.UnmetPrecondition)
        {
            string errorMessage = result.ErrorReason switch
            {
                "Invalid context for command; accepted contexts: Guild."
                    => "Command can only be used in a server.",

                _ => result.ErrorReason
            };

            await ctx.Interaction.RespondAsync(
                errorMessage,
                ephemeral: true);

            logger.LogWarning("{ErrorReason}", result.ErrorReason);
        }
    }
}

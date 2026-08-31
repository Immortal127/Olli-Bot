using Discord;
using Discord.Interactions;
using MediatR;

namespace OlliBot.Bot.Notifications.Handlers;
internal class SlashCommandExecutedHandler(ILogger<SlashCommandExecutedHandler> logger) : INotificationHandler<CommandExecutedNotification>
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
            _ => "Unknown"
        };

        if (result.IsSuccess)
        {
            logger.LogInformation("{Type} Command '{Command}' completed without InteractionService errors", commandType, commandInfo.Name);
        }
        else if (!result.IsSuccess)
        {
            if (result.Error == InteractionCommandError.UnmetPrecondition)
            {
                if (result.ErrorReason == "Invalid context for command; accepted contexts: Guild.")
                {
                    await ctx.Interaction.RespondAsync("Command can only be used in a server.", ephemeral: true);
                    logger.LogWarning("{ErrorReason}", result.ErrorReason);
                }
                if (result.Error == InteractionCommandError.UnmetPrecondition)
                {
                    await ctx.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
                    logger.LogWarning("{ErrorReason}", result.ErrorReason);
                }
            }
        }
    }
}

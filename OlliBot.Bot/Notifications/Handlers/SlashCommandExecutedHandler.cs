using Discord;
using Discord.Interactions;
using MediatR;

namespace OlliBot.Bot.Notifications.Handlers;
internal class SlashCommandExecutedHandler(ILogger<SlashCommandExecutedHandler> logger) : INotificationHandler<SlashCommandExecutedNotification>
{
    public async Task Handle(
        SlashCommandExecutedNotification notification,
        CancellationToken cancellationToken)
    {
        IResult result = notification.Result;
        IInteractionContext ctx = notification.InteractionContext;
        SlashCommandInfo slashInfo = notification.SlashCommandInfo;

        if (result.IsSuccess)
        {
            logger.LogInformation("Command '{Command}' completed without InteractionService errors", slashInfo.Name);
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

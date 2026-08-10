using Discord.Interactions;
using Discord.WebSocket;
using MediatR;

namespace OlliBot.Bot.Notifications.Handlers;
internal class ExecuteCommandHandler(
    InteractionService interactionService,
    DiscordSocketClient client,
    IServiceProvider serviceProvider,
    ILogger<ExecuteCommandHandler> logger) : INotificationHandler<InteractionCreatedNotification>
{
    public async Task Handle(InteractionCreatedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var context = new SocketInteractionContext(client, notification.Interaction);
            await interactionService.ExecuteCommandAsync(context, serviceProvider);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error handling interaction.");
        }
    }
}

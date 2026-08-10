using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using OlliBot.Bot.Notifications;

namespace OlliBot.Bot;

public class DiscordEventListener(DiscordSocketClient client, IServiceScopeFactory serviceScope)
{
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

    private IMediator Mediator
    {
        get
        {
            IServiceScope scope = serviceScope.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IMediator>();
        }
    }

    internal Task OnMessageReceivedAsync(SocketMessage message)
    {
        return Mediator.Publish(new MessageReceivedNotification(message), _cancellationToken);
    }

    internal Task OnInteractionCreated(SocketInteraction interaction)
    {
        return Mediator.Publish(new InteractionCreatedNotification(interaction), _cancellationToken);
    }

    internal Task OnClientReady()
    {
        return Mediator.Publish(new ClientReadyNotification(), _cancellationToken);
    }

    internal Task OnSlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
    {
        return Mediator.Publish(new SlashCommandExecutedNotification(info, context, result), _cancellationToken);
    }
}

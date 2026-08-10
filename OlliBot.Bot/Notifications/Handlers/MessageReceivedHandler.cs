using Discord;
using Discord.WebSocket;
using MediatR;
using OlliBot.Bot.Utilities;

namespace OlliBot.Bot.Notifications.Handlers;

public class MessageReceivedHandler(
    DiscordSocketClient client,
    IConfiguration configuration) : INotificationHandler<MessageReceivedNotification>
{
    public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        SocketMessage message = notification.Message;

        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            return;
        }

        var channel = (ITextChannel)message.Channel;
        IGuild guild = channel.Guild;

        if (!message.IsAuthorOlliBot(client) && message.Content.Contains("good bot", StringComparison.OrdinalIgnoreCase) && (await channel.GetMessagesAsync(message.Id, Direction.Before, 10).FlattenAsync()).Any(m => m.IsAuthorOlliBot(client)))
        {
            await channel.SendMessageAsync(":3", messageReference: new MessageReference(message.Id));
        }
        if (message.ContentExeedsLength(150) && !message.IsAuthorOlliBot(client))
        {
            await channel.SendMessageAsync("i ain't reading all that", messageReference: new MessageReference(message.Id));
            await channel.SendMessageAsync("i'm happy for u tho");
            await channel.SendMessageAsync("or sorry that happened");
            return;
        }

        if (message.Author.Id == 164740251934392321 && guild.Id.ToString() == configuration["MainServer"] && new Random().Next(1, 101) <= 15)
        {
            await channel.SendMessageAsync("James Here", messageReference: new MessageReference(message.Id));
        }
    }
}

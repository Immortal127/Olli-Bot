using Discord.WebSocket;
using OlliBot.Application.HumbleBundle;
using OlliBot.Domain.Entities;

namespace OlliBot.Bot.Services;
public class DiscordSubscriberValidater(DiscordSocketClient client) : IDiscordSubscriberValidater
{
    public IEnumerable<HumbleBundleSubscriber> FindStaleSubscribers(IEnumerable<HumbleBundleSubscriber> subscribers)
    {
        HashSet<ulong> channelIds = client.Guilds
            .SelectMany(guild => guild.Channels)
            .Select(channel => channel.Id)
            .ToHashSet();

        var staleSubscribers = subscribers.Where(s => channelIds.Add(s.DiscordId));

        return staleSubscribers;
    }
}

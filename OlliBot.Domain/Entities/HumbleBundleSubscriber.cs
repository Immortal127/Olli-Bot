using OlliBot.Domain.Enums;

namespace OlliBot.Domain.Entities;

public class HumbleBundleSubscriber
{
    public int Id { get; set; }
    public ulong DiscordId { get; set; }
    public ulong? GuildId { get; set; }
    public HumbleBundleSubscriberType SubscriberType { get; set; }
    public HumbleBundleType SubscriptionType { get; set; }
}

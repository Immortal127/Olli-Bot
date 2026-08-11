using OlliBot.Domain.Entities;

namespace OlliBot.Application.HumbleBundle;
public interface IDiscordSubscriberValidater
{
    IEnumerable<HumbleBundleSubscriber> FindStaleSubscribers(IEnumerable<HumbleBundleSubscriber> subscribers);
}
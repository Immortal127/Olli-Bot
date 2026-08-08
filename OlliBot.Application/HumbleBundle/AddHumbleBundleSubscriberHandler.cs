using Microsoft.Extensions.Logging;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.HumbleBundle;
public class AddHumbleBundleSubscriberHandler(ILogger<AddHumbleBundleSubscriberHandler> logger, IHumbleBundleRepository humbleBundleRepository)
{
    public async Task<AddHumbleBundleSubscriberResult> HandleAsync(AddHumbleBundleSubscriberCommand command, CancellationToken ct = default)
    {
        var subscriber = new HumbleBundleSubscriber
        {
            DiscordId = command.SubscriberId,
            SubscriptionType = command.HumbleBundleType,
            SubscriberType = command.HumbleBundleSubscriberType,
            GuildId = command.GuildId,
            RoleId = command.RoleId
        };

        await humbleBundleRepository.AddSubscriberAsync(subscriber, ct);

        return new AddHumbleBundleSubscriberResult(true, "Subscriber added successfully.");
    }
}

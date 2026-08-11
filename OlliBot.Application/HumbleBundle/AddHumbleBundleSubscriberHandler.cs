using MediatR;
using Microsoft.Extensions.Logging;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.HumbleBundle;
public class AddHumbleBundleSubscriberHandler(
    ILogger<AddHumbleBundleSubscriberHandler> logger,
    IHumbleBundleRepository humbleBundleRepository) : IRequestHandler<AddHumbleBundleSubscriberCommand, AddHumbleBundleSubscriberResult>
{
    public async Task<AddHumbleBundleSubscriberResult> Handle(AddHumbleBundleSubscriberCommand command, CancellationToken ct = default)
    {
        var subscriber = new HumbleBundleSubscriber
        {
            DiscordId = command.SubscriberId,
            SubscriptionType = command.HumbleBundleType,
            SubscriberType = command.HumbleBundleSubscriberType,
            GuildId = command.GuildId,
            RoleId = command.RoleId
        };

        if (await humbleBundleRepository.SubscriberExistsAsync(subscriber.DiscordId, subscriber.SubscriptionType, ct))
        {
            logger.LogInformation("Subscriber with DiscordId {DiscordId} and SubscriptionType {SubscriptionType} already exists.", subscriber.DiscordId, subscriber.SubscriptionType);
            return new AddHumbleBundleSubscriberResult(false, "Subscriber already exists.");
        }

        await humbleBundleRepository.AddSubscriberAsync(subscriber, ct);

        return new AddHumbleBundleSubscriberResult(true, "Subscriber added successfully.");
    }
}

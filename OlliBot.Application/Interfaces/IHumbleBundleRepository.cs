using OlliBot.Domain.Entities;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.Interfaces;
public interface IHumbleBundleRepository
{
    Task AddBundlesAsync(IReadOnlyList<Domain.Entities.HumbleBundle> bundles, CancellationToken ct);

    Task<IReadOnlyList<Domain.Entities.HumbleBundle>> GetCurrentBundlesAsync(HumbleBundleType bundleType, CancellationToken ct);

    Task<IReadOnlyList<HumbleBundleSubscriber>> GetSubscribersAsync(HumbleBundleType bundleType, CancellationToken ct);

    Task AddSubscriberAsync(HumbleBundleSubscriber subscriber, CancellationToken ct);

    Task DeleteStaleChannelSubscribersAsync(IEnumerable<HumbleBundleSubscriber> subscribers, CancellationToken ct);

    Task DeleteBundlesAsync(IReadOnlyList<Domain.Entities.HumbleBundle> bundles, CancellationToken ct);

    Task<bool> SubscriberExistsAsync(ulong discordId, HumbleBundleType subscriptionType, CancellationToken ct);
}

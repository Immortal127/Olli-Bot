using OlliBot.Domain.Entities;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.Interfaces;
public interface IHumbleBundleRepository
{
    Task AddBundlesAsync(IReadOnlyList<Domain.Entities.HumbleBundle> bundles, CancellationToken ct);

    Task<IReadOnlyList<Domain.Entities.HumbleBundle>> GetCurrentBundlesAsync(HumbleBundleType bundleType);

    Task<IReadOnlyList<HumbleBundleSubscriber>> GetCurrentSubscribersAsync(HumbleBundleType bundleType);

    Task AddSubscriberAsync(HumbleBundleSubscriber subscriber, CancellationToken ct);

    Task DeleteStaleSubscribersAsync(CancellationToken ct);

    Task DeleteExpiredBundlesAsync(IReadOnlyList<Domain.Entities.HumbleBundle> bundles, CancellationToken ct);
}

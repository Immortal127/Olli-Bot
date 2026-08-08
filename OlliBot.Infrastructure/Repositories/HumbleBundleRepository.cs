using Microsoft.EntityFrameworkCore;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;
using OlliBot.Domain.Enums;
using OlliBot.Infrastructure.Data;

namespace OlliBot.Infrastructure.Repositories;
public class HumbleBundleRepository(OlliBotDbContext db) : IHumbleBundleRepository
{
    public Task DeleteExpiredBundlesAsync(IReadOnlyList<Domain.Entities.HumbleBundle> bundles, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStaleSubscribersAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Domain.Entities.HumbleBundle>> GetCurrentBundlesAsync(HumbleBundleType bundleType)
    {
        return await db.HumbleBundles
            .AsNoTracking()
            .Where(hb => hb.BundleType == bundleType)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<HumbleBundleSubscriber>> GetCurrentSubscribersAsync(HumbleBundleType bundleType)
    {
        return await db.HumbleBundleSubscribers
            .AsNoTracking()
            .Where(sub => sub.SubscriptionType == bundleType)
            .ToListAsync();
    }

    public async Task AddBundlesAsync(IReadOnlyList<Domain.Entities.HumbleBundle> bundles, CancellationToken ct)
    {
        await db.HumbleBundles.AddRangeAsync(bundles, ct);
    }

    public async Task AddSubscriberAsync(HumbleBundleSubscriber subscriber, CancellationToken ct)
    {
        await db.HumbleBundleSubscribers.AddAsync(subscriber, ct);
        await db.SaveChangesAsync(ct);
    }
}

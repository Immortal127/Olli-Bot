using MediatR;
using Microsoft.Extensions.Logging;
using OlliBot.Application.Interfaces;

namespace OlliBot.Application.HumbleBundle;
public class GetUserHumbleBundleSubscriptionsHandler(
    ILogger<GetUserHumbleBundleSubscriptionsHandler> logger,
    IHumbleBundleRepository humbleBundleRepository) : IRequestHandler<GetUserHumbleBundleSubscriptionsQuery, GetUserHumbleBundleSubscriptionsResult>
{
    public async Task<GetUserHumbleBundleSubscriptionsResult> Handle(GetUserHumbleBundleSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var humbleBundleTypes = await humbleBundleRepository.GetSubscriptions(request.DiscordId, cancellationToken);

        return new GetUserHumbleBundleSubscriptionsResult(humbleBundleTypes.Select(sub => sub.SubscriptionType).ToList(), humbleBundleTypes.Any());
    }
}

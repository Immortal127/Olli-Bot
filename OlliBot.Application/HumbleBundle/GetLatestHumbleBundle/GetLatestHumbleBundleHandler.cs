using MediatR;
using Microsoft.Extensions.Logging;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.HumbleBundle.GetLatestHumbleBundle;
public class GetLatestHumbleBundleHandler(
    ILogger<GetLatestHumbleBundleHandler> logger,
    IHumbleBundleRepository humbleBundleRepository,
    IHumbleBundleScanner humbleBundleScanner) : IRequestHandler<GetLatestHumbleBundleQuery, GetLatestHumbleBundleResult>
{
    public async Task<GetLatestHumbleBundleResult> Handle(GetLatestHumbleBundleQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.HumbleBundle? bundle = await humbleBundleRepository.GetLatestBundle(request.BundleType);

        if (bundle is null)
        {
            logger.LogWarning("No bundle found for type {BundleType}", request.BundleType);
            return new GetLatestHumbleBundleResult(null, false, $"No bundle found for type {request.BundleType}");
        }

        var latestBundle = await humbleBundleScanner.GetBundleDetails(bundle, cancellationToken);

        return new GetLatestHumbleBundleResult(latestBundle, true, $"Successfully retrieved latest bundle for type {request.BundleType}");
    }
}

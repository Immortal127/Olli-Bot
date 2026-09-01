using MediatR;
using Microsoft.Extensions.Logging;
using OlliBot.Application.Interfaces;
using OlliBot.Bot.Modules.HumbleBundle;

namespace OlliBot.Application.HumbleBundle;
public class RemoveHumbleBundleSubscriberHandler(ILogger<RemoveHumbleBundleSubscriberHandler> logger, IHumbleBundleRepository humbleBundleRepository) : IRequestHandler<RemoveHumbleBundleSubscriberCommand, RemoveHumbleBundleSubscriberResult>
{
    public async Task<RemoveHumbleBundleSubscriberResult> Handle(RemoveHumbleBundleSubscriberCommand request, CancellationToken cancellationToken)
    {
        int deletedCount = await humbleBundleRepository.RemoveSubscriberAsync(
            request.DiscordId,
            request.HumbleBundleType,
            request.SubscriberType,
            cancellationToken);

        return new RemoveHumbleBundleSubscriberResult(deletedCount > 0, deletedCount > 0 ? "Success" : "Failure");
    }
}

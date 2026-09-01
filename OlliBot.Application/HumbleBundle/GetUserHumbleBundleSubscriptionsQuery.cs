using MediatR;

namespace OlliBot.Application.HumbleBundle;
public record GetUserHumbleBundleSubscriptionsQuery(ulong DiscordId) : IRequest<GetUserHumbleBundleSubscriptionsResult>;
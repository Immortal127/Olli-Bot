using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.RemoveHumbleBundleSubscriber;

public record RemoveHumbleBundleSubscriberCommand(HumbleBundleType HumbleBundleType, ulong DiscordId, HumbleBundleSubscriberType SubscriberType) : IRequest<RemoveHumbleBundleSubscriberResult>;
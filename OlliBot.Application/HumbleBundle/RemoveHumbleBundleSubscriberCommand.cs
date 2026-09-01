using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Modules.HumbleBundle;

public record RemoveHumbleBundleSubscriberCommand(HumbleBundleType HumbleBundleType, ulong DiscordId, HumbleBundleSubscriberType SubscriberType) : IRequest<RemoveHumbleBundleSubscriberResult>;
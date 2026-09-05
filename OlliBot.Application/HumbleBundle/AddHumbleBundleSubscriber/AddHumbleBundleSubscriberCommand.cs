using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.AddHumbleBundleSubscriber;
public record AddHumbleBundleSubscriberCommand(
    HumbleBundleType HumbleBundleType,
    ulong SubscriberId,
    HumbleBundleSubscriberType HumbleBundleSubscriberType,
    ulong? GuildId = null,
    ulong? RoleId = null) : IRequest<AddHumbleBundleSubscriberResult>;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle;
public record GetUserHumbleBundleSubscriptionsResult(IReadOnlyCollection<HumbleBundleType> HumbleBundleTypes, bool Success);
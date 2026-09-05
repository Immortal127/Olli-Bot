using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.HumbleBundle.CheckForHumbleBundleUpdates;
public record CheckForHumbleBundleUpdatesResult(
    bool Success,
    string Message,
    IEnumerable<ScannedHumbleBundle> ScannedBundles,
    IEnumerable<HumbleBundleSubscriber> Subscribers);
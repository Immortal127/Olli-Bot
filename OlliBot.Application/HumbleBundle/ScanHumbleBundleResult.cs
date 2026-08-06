using OlliBot.Application.HumbleBundle.Models;

namespace OlliBot.Application.HumbleBundle;
public record ScanHumbleBundleResult(IEnumerable<ScannedHumbleBundle> ScannedBundles, bool Success, string? Message);
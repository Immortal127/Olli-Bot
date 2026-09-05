using OlliBot.Application.HumbleBundle.Models;

namespace OlliBot.Application.HumbleBundle.ScanHumbleBundle;
public record ScanHumbleBundleResult(IEnumerable<ScannedHumbleBundle> ScannedBundles, bool Success, string? Message);
using OlliBot.Application.HumbleBundle.Models;

namespace OlliBot.Application.HumbleBundle.GetLatestHumbleBundle;
public record GetLatestHumbleBundleResult(ScannedHumbleBundle? Bundle, bool Success, string Message);
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle;
public interface IHumbleBundleScanner
{
    Task<IReadOnlyCollection<ScannedHumbleBundle>> ScanAsync(HumbleBundleType bundleType, CancellationToken ct);
}

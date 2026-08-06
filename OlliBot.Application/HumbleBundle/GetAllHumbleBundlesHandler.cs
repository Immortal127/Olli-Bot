using OlliBot.Application.HumbleBundle.Models;

namespace OlliBot.Application.HumbleBundle;

public class GetAllHumbleBundlesHandler(IHumbleBundleScanner humbleBundleScanner)
{
    public async Task<ScanHumbleBundleResult> HandleAsync(ScanHumbleBundleCommand command, CancellationToken ct = default)
    {
        IEnumerable<ScannedHumbleBundle> scanResult = await humbleBundleScanner.ScanAsync(command.BundleType, ct);

        return new ScanHumbleBundleResult(scanResult, true, null);
    }
}

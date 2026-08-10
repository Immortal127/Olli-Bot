using MediatR;
using OlliBot.Application.HumbleBundle.Models;

namespace OlliBot.Application.HumbleBundle;

public class GetAllHumbleBundlesHandler(IHumbleBundleScanner humbleBundleScanner) : IRequestHandler<ScanHumbleBundleCommand, ScanHumbleBundleResult>
{
    public async Task<ScanHumbleBundleResult> Handle(ScanHumbleBundleCommand command, CancellationToken ct = default)
    {
        IEnumerable<ScannedHumbleBundle> scanResult = await humbleBundleScanner.ScanAsync(command.BundleType, ct);

        return new ScanHumbleBundleResult(scanResult, true, null);
    }
}

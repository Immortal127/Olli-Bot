namespace OlliBot.Application.EmoteRanking.Scanning;
public interface IEmoteCountScanner
{
    Task<EmoteScanResult> ScanAsync(EmoteScanRequest request, CancellationToken ct);
}

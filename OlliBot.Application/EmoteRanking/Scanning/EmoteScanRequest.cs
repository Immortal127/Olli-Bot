namespace OlliBot.Application.EmoteRanking.Scanning;
public record EmoteScanRequest(
    ulong GuildId,
    IReadOnlyDictionary<ulong, ulong> StartingCheckpoints);
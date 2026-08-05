namespace OlliBot.Application.EmoteRanking.Scanning;

public sealed record EmoteScanResult(
    // EmoteId → newly discovered count
    IReadOnlyDictionary<ulong, int> NewCounts,

    // ChannelId → latest scanned MessageId
    IReadOnlyDictionary<ulong, ulong> UpdatedCheckpoints);
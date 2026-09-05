namespace OlliBot.Application.EmoteRanking.UpdateEmoteRanking;
public record UpdateEmoteRankingResult(
    bool Success,
    IReadOnlyDictionary<ulong, int>? Counts,
    string Message);

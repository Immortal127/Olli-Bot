namespace OlliBot.Application.EmoteRanking.Commands;
public record UpdateEmoteRankingResult(
    bool Success,
    IReadOnlyDictionary<ulong, int>? Counts,
    string Message);

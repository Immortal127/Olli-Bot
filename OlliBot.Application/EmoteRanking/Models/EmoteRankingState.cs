namespace OlliBot.Application.EmoteRanking.Models;

public sealed record EmoteRankingState(
    ulong GuildId,
    IReadOnlyDictionary<ulong, int> Counts,
    IReadOnlyDictionary<ulong, ulong> ChannelCheckpoints);
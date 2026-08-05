using OlliBot.Application.EmoteRanking.Models;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.Interfaces;
public interface IEmoteCountRepository
{
    Task<EmoteCount> GetEmoteCount(ulong emoteId, CancellationToken ct);

    Task ClearGuildStateAsync(ulong guildId, CancellationToken ct);

    Task<int> DeleteStaleCountsAsync(ulong guildId, IReadOnlyCollection<ulong> activeEmoteIds, CancellationToken ct);

    Task<LastChannelMessage?> GetLastMessageForChannel(ulong guildId, ulong channelId, CancellationToken ct);

    Task<IEnumerable<LastChannelMessage>> GetLastMessagesForGuild(ulong guildId, CancellationToken ct);

    Task<Dictionary<ulong, int>> GetCountsAsync(ulong guildId, CancellationToken ct);

    Task<EmoteRankingState> GetGuildStateAsync(ulong guildId, CancellationToken ct);

    Task SaveAsync(EmoteRankingState updatedState, DateTime utcNow, CancellationToken ct);
}
namespace OlliBot.Application.Interfaces;
public interface IEmoteCountRepository
{
    Task<Dictionary<ulong, int>> GetCountsAsync(ulong guildId);

    Task SaveCountsAsync(ulong guildId, Dictionary<ulong, int> counts);

    Task DeleteStaleAsync(ulong guildId, HashSet<ulong> activeEmoteIds);
}
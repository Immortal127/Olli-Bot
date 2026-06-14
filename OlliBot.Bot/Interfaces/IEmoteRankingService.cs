using Discord;
using OlliBot.Infrastructure.Entities;

namespace OlliBot.Bot.Interfaces;

public interface IEmoteRankingService
{
    Task DeleteStaleEmoteCounts(HashSet<ulong> activeEmoteIds, IInteractionContext context);
    Task<Dictionary<GuildEmote, int>> GetEmoteCounts(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<ITextChannel> textChannels, IInteractionContext context);
    Task<Dictionary<GuildEmote, int>> GetNewEmoteCounts(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<ITextChannel> textChannels, IEnumerable<LastChannelMessage> lastMessages);
    Task ResetDB(IInteractionContext context);
    Task UpdateOrAddEmoteCounts(Dictionary<GuildEmote, int> emoteCounts, IInteractionContext context);
    Task UpdateOrAddLastMessage(ITextChannel channel, IMessage message);
}
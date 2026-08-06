using OlliBot.Application.Interfaces;

namespace OlliBot.Application.EmoteRanking.Commands;

public class ClearEmoteRankingHandler(IEmoteCountRepository emoteCountRepository)
{
    public async Task<ClearEmoteRankingResult> HandleAsync(ClearEmoteRankingCommand command, CancellationToken ct = default)
    {
        if (!command.IsInvokedByAdmin)
        {
            return new ClearEmoteRankingResult(false, "You do not have permission to clear emote rankings.");
        }

        await emoteCountRepository.ClearGuildStateAsync(command.GuildId, ct);

        return new ClearEmoteRankingResult(true, $"Emote ranking for guild {command.GuildId} has been cleared.");
    }
}

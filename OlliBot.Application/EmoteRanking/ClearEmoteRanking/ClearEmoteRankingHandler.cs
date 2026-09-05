using MediatR;

namespace OlliBot.Application.EmoteRanking.ClearEmoteRanking;

public class ClearEmoteRankingHandler(IEmoteCountRepository emoteCountRepository) : IRequestHandler<ClearEmoteRankingCommand, ClearEmoteRankingResult>
{
    public async Task<ClearEmoteRankingResult> Handle(ClearEmoteRankingCommand command, CancellationToken ct = default)
    {
        if (!command.IsInvokedByAdmin)
        {
            return new ClearEmoteRankingResult(false, "You do not have permission to clear emote rankings.");
        }

        await emoteCountRepository.ClearGuildStateAsync(command.GuildId, ct);

        return new ClearEmoteRankingResult(true, $"Emote ranking for guild {command.GuildId} has been cleared.");
    }
}

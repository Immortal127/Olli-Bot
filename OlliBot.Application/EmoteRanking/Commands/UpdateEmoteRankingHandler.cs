using MediatR;
using OlliBot.Application.EmoteRanking.Models;
using OlliBot.Application.EmoteRanking.Scanning;
using OlliBot.Application.Interfaces;

namespace OlliBot.Application.EmoteRanking.Commands;
public class UpdateEmoteRankingHandler(IEmoteCountRepository repository, IEmoteCountScanner emoteScanner) : IRequestHandler<UpdateEmoteRankingCommand, UpdateEmoteRankingResult>
{
    public async Task<UpdateEmoteRankingResult> Handle(UpdateEmoteRankingCommand command, CancellationToken ct = default)
    {
        try
        {
            EmoteRankingState existingState = await repository
                .GetGuildStateAsync(command.GuildId, ct);

            var scanRequest = new EmoteScanRequest(
                GuildId: command.GuildId,
                StartingCheckpoints:
                    existingState.ChannelCheckpoints);

            EmoteScanResult scanResult =
                await emoteScanner.ScanAsync(
                    scanRequest,
                    ct);

            // 3. Combine existing counts with newly discovered counts.
            //
            // NewCounts contains only currently active Discord emotes,
            // so stale stored emotes are intentionally excluded.
            Dictionary<ulong, int> combinedCounts = [];

            foreach ((ulong emoteId, int newCount) in
                     scanResult.NewCounts)
            {
                existingState.Counts.TryGetValue(
                    emoteId,
                    out int existingCount);

                combinedCounts[emoteId] =
                    existingCount + newCount;
            }

            /*
             * UpdatedCheckpoints contains the current guild channels.
             * Checkpoints belonging to deleted channels are therefore
             * excluded from the new state.
             */
            var updatedState = new EmoteRankingState(
                GuildId: command.GuildId,
                Counts: combinedCounts,
                ChannelCheckpoints:
                    scanResult.UpdatedCheckpoints);

            // 4. Save counts and checkpoints together.
            await repository.SaveAsync(
                updatedState,
                DateTime.UtcNow,
                ct);

            return new UpdateEmoteRankingResult(
                true,
                combinedCounts,
                "Emote rankings updated.");
        }
        catch (Exception ex)
        {
            return new UpdateEmoteRankingResult(false, null, "Emote ranking failed");
        }
    }
}
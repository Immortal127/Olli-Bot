using Microsoft.EntityFrameworkCore;
using OlliBot.Application.EmoteRanking.Models;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;
using OlliBot.Infrastructure.Data;

namespace OlliBot.Infrastructure.Repositories;
internal class EmoteCountRepository(OlliBotDbContext db) : IEmoteCountRepository
{
    public async Task ClearGuildStateAsync(ulong guildId, CancellationToken ct)
    {
        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(ct);

        await db.EmoteCounts
            .Where(e => e.GuildId == guildId)
            .ExecuteDeleteAsync(ct);

        await db.LastChannelMessages
            .Where(e => e.GuildId == guildId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }

    public async Task<LastChannelMessage?> GetLastMessageForChannel(ulong guildId, ulong channelId, CancellationToken ct)
    {
        return await db.LastChannelMessages
            .AsNoTracking()
            .SingleOrDefaultAsync(
                message =>
                    message.GuildId == guildId &&
                    message.ChannelId == channelId,
                ct);
    }

    public async Task<IEnumerable<LastChannelMessage>> GetLastMessagesForGuild(ulong guildId, CancellationToken ct)
    {
        return await db.LastChannelMessages
            .AsNoTracking()
            .Where(message => message.GuildId == guildId)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<ulong, int>> GetCountsAsync(ulong guildId, CancellationToken ct)
    {
        return await db.EmoteCounts.AsNoTracking().Where(x => x.GuildId == guildId).ToDictionaryAsync(x => x.EmoteId, x => x.Count, cancellationToken: ct);
    }

    public async Task<int> DeleteStaleCountsAsync(
        ulong guildId,
        IReadOnlyCollection<ulong> activeEmoteIds,
        CancellationToken ct)
    {
        return await db.EmoteCounts
            .Where(count =>
                count.GuildId == guildId &&
                !activeEmoteIds.Contains(count.EmoteId))
            .ExecuteDeleteAsync(ct);
    }

    public async Task<EmoteCount> GetEmoteCount(ulong emoteId, CancellationToken ct)
    {
        EmoteCount? existingEntry = await db.EmoteCounts.SingleOrDefaultAsync(e => e.EmoteId == emoteId, ct);
        return existingEntry ?? throw new InvalidOperationException($"EmoteCount with ID {emoteId} not found.");
    }

    public async Task<EmoteRankingState> GetGuildStateAsync(
        ulong guildId,
        CancellationToken ct)
    {
        Dictionary<ulong, int> counts = await db.EmoteCounts
            .Where(count => count.GuildId == guildId)
            .ToDictionaryAsync(
                count => count.EmoteId,
                count => count.Count,
                ct);

        Dictionary<ulong, ulong> checkpoints = await db.LastChannelMessages
            .Where(message => message.GuildId == guildId)
            .ToDictionaryAsync(
                message => message.ChannelId,
                message => message.MessageId,
                ct);

        return new EmoteRankingState(
            GuildId: guildId,
            Counts: counts,
            ChannelCheckpoints: checkpoints);
    }

    public async Task SaveAsync(
        EmoteRankingState state,
        DateTime updatedAtUtc,
        CancellationToken ct)
    {
        List<EmoteCount> storedCounts =
            await db.EmoteCounts
                .Where(count =>
                    count.GuildId == state.GuildId)
                .ToListAsync(ct);

        List<LastChannelMessage> storedCheckpoints =
            await db.LastChannelMessages
                .Where(checkpoint =>
                    checkpoint.GuildId == state.GuildId)
                .ToListAsync(ct);

        SynchronizeCounts(
            state,
            storedCounts,
            updatedAtUtc);

        SynchronizeCheckpoints(
            state,
            storedCheckpoints);

        await db.SaveChangesAsync(ct);
    }

    private void SynchronizeCounts(
    EmoteRankingState state,
    List<EmoteCount> storedCounts,
    DateTime updatedAtUtc)
    {
        var storedByEmoteId =
            storedCounts.ToDictionary(
                count => count.EmoteId);

        IEnumerable<EmoteCount> staleCounts =
            storedCounts.Where(
                count =>
                    !state.Counts.ContainsKey(
                        count.EmoteId));

        db.EmoteCounts.RemoveRange(staleCounts);

        foreach ((ulong emoteId, int count) in state.Counts)
        {
            if (storedByEmoteId.TryGetValue(
                    emoteId,
                    out EmoteCount? stored))
            {
                stored.Count = count;
                stored.DateTimeUpdated = updatedAtUtc;
            }
            else
            {
                db.EmoteCounts.Add(
                    new EmoteCount
                    {
                        GuildId = state.GuildId,
                        EmoteId = emoteId,
                        Count = count,
                        DateTimeUpdated = updatedAtUtc
                    });
            }
        }
    }

    private void SynchronizeCheckpoints(
        EmoteRankingState state,
        List<LastChannelMessage> storedCheckpoints)
    {
        var storedByChannelId =
            storedCheckpoints.ToDictionary(
                checkpoint => checkpoint.ChannelId);

        IEnumerable<LastChannelMessage> staleCheckpoints =
            storedCheckpoints.Where(
                checkpoint =>
                    !state.ChannelCheckpoints.ContainsKey(
                        checkpoint.ChannelId));

        db.LastChannelMessages.RemoveRange(
            staleCheckpoints);

        foreach ((ulong channelId, ulong messageId) in
                 state.ChannelCheckpoints)
        {
            if (storedByChannelId.TryGetValue(
                    channelId,
                    out LastChannelMessage? stored))
            {
                stored.MessageId = messageId;
            }
            else
            {
                db.LastChannelMessages.Add(
                    new LastChannelMessage
                    {
                        GuildId = state.GuildId,
                        ChannelId = channelId,
                        MessageId = messageId
                    });
            }
        }
    }
}
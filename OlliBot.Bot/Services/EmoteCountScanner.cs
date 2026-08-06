using Discord;
using OlliBot.Application.EmoteRanking.Scanning;

namespace OlliBot.Bot.Services;

public sealed class EmoteCountScanner(
    IDiscordClient discordClient,
    ILogger<EmoteCountScanner> logger)
    : IEmoteCountScanner
{
    public async Task<EmoteScanResult> ScanAsync(
        EmoteScanRequest request,
        CancellationToken ct)
    {
        IGuild guild =
            await discordClient.GetGuildAsync(request.GuildId)
            ?? throw new InvalidOperationException(
                $"Guild with ID {request.GuildId} was not found.");

        Dictionary<ulong, GuildEmote> guildEmotes = guild.Emotes.ToDictionary(emote => emote.Id);

        ITextChannel[] textChannels = (await guild.GetTextChannelsAsync()).ToArray();

        Dictionary<ulong, int> newCounts = guildEmotes.Keys.ToDictionary(emoteId => emoteId, _ => 0);

        Dictionary<ulong, ulong> updatedCheckpoints = [];

        var requestOptions = new RequestOptions
        {
            CancelToken = ct
        };

        foreach (ITextChannel channel in textChannels)
        {
            ct.ThrowIfCancellationRequested();

            logger.LogDebug(
                "Scanning emote usage in channel {ChannelId} for guild {GuildId}",
                channel.Id,
                request.GuildId);

            ulong cursorMessageId =
                request.StartingCheckpoints.TryGetValue(
                    channel.Id,
                    out ulong checkpointMessageId)
                        ? checkpointMessageId
                        : 0;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                List<IMessage> messages =
                    (await channel
                        .GetMessagesAsync(
                            cursorMessageId,
                            Direction.After,
                            100,
                            CacheMode.AllowDownload,
                            requestOptions)
                        .FlattenAsync())
                    .ToList();

                // Discord returns the page newest-to-oldest, so we reverse the order.
                messages.Reverse();

                if (messages.Count == 0)
                {
                    break;
                }

                foreach (IMessage message in messages)
                {
                    // Skip messages sent by the bot itself
                    if (message.Author.Id == discordClient.CurrentUser.Id)
                    {
                        continue;
                    }

                    foreach ((ulong emoteId, GuildEmote emote) in guildEmotes)
                    {
                        bool usedInContent =
                            message.Content.Contains(
                                emote.ToString(),
                                StringComparison.Ordinal);

                        bool usedAsReaction =
                            message.Reactions.Any(
                                reaction =>
                                    reaction.Key.Equals(emote));

                        if (usedInContent || usedAsReaction)
                        {
                            newCounts[emoteId]++;
                        }
                    }
                }

                cursorMessageId = messages[^1].Id;
            }

            if (cursorMessageId != 0)
            {
                updatedCheckpoints[channel.Id] = cursorMessageId;
            }
        }

        return new EmoteScanResult(
            newCounts,
            updatedCheckpoints);
    }
}
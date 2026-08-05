using Discord;
using Discord.WebSocket;
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

        Dictionary<ulong, int> newCounts = guild.Emotes.ToDictionary(emote => emote.Id, _ => 0);

        Dictionary<ulong, ulong> updatedCheckpoints = [];

        foreach (ITextChannel channel in textChannels)
        {
            ct.ThrowIfCancellationRequested();

            logger.LogDebug(
                "Scanning emote usage in channel {ChannelId} for guild {GuildId}",
                channel.Id,
                request.GuildId);

            IMessage? lastMessage = null;

            if (request.StartingCheckpoints.TryGetValue(
                    channel.Id,
                    out ulong checkpointMessageId))
            {
                lastMessage =
                    await channel.GetMessageAsync(
                        checkpointMessageId);

                /*
                 * Do not silently scan from the beginning if the saved
                 * checkpoint was deleted. Existing counts would then be
                 * combined with the complete message history, causing
                 * duplicate counts.
                 */
                if (lastMessage is null)
                {
                    throw new InvalidOperationException(
                        $"Checkpoint message {checkpointMessageId} " +
                        $"for channel {channel.Id} no longer exists. " +
                        "The rankings must be rebuilt.");
                }
            }

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                IEnumerable<IMessage> fetchedMessages =
                    await (
                        lastMessage is null
                            ? channel
                                .GetMessagesAsync(
                                    0,
                                    Direction.After,
                                    100)
                                .FlattenAsync()
                            : channel
                                .GetMessagesAsync(
                                    lastMessage,
                                    Direction.After,
                                    100)
                                .FlattenAsync());

                List<IMessage> messages =
                    fetchedMessages.ToList();

                // Discord returns the page newest-to-oldest.
                messages.Reverse();

                if (messages.Count == 0)
                {
                    break;
                }

                foreach (IMessage message in messages)
                {
                    if (message.Author.Id ==
                        discordClient.CurrentUser.Id)
                    {
                        continue;
                    }

                    foreach ((ulong emoteId, GuildEmote emote) in
                             guildEmotes)
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

                lastMessage = messages[^1];
            }

            if (lastMessage is not null)
            {
                updatedCheckpoints[channel.Id] =
                    lastMessage.Id;
            }
        }

        return new EmoteScanResult(
            newCounts,
            updatedCheckpoints);
    }
}
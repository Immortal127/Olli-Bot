using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using OlliBot.Data;
using OlliBot.Modules;
using System.Linq;

namespace OlliBot.Services
{
    
    public interface IEmoteRankingService
    {
        public Task<Dictionary<GuildEmote, int>> GetEmoteCounts(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<ITextChannel> textChannels, IInteractionContext context);
        public Task ResetDB(IInteractionContext context);
        /*public Task<Dictionary<GuildEmote, int>> GetLastMessage(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<ITextChannel> textChannels, IInteractionContext context);
        
        public Task AddLastMessageToDB(ITextChannel channel, IMessage message);

        public Task AddEmoteCountToDB(GuildEmote guildEmote, int count);

        public Task DeleteOldLastMessage(ITextChannel channel, IMessage message);

        public Task DeleteOldEmoteCount(GuildEmote guildEmote, int count);

        public Task UpdateLastMessage(ITextChannel channel, IMessage message);

        public Task UpdateEmoteCount(GuildEmote guildEmote, int count);
        public bool CheckForOldEmotes#*/
    }
        

    /*
     GetEmoteCounts is entry point for the discord command
    get all emotes and textable channels in server
    get current emote counts from database
    get last message checked for every channel in server from database


    */
    public class EmoteRankingService : IEmoteRankingService
    {
        //Entry point for discord command
        public async Task<Dictionary<GuildEmote, int>> GetEmoteCounts(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<ITextChannel> textChannels, IInteractionContext context)
        {
            DateTime dateTimeExecuted = DateTime.UtcNow;


            using var db = new EmoteRankingDB();

            Dictionary<ulong, int> recordedEmoteCounts = db.EmoteCounts.Where(x => x.GuildId == context.Guild.Id).ToDictionary(x => x.EmoteId, x => x.Count);
            IEnumerable<LastChannelMessage> lastChannelMessages = await db.LastChannelMessages.Where(x => x.GuildId == context.Guild.Id).ToListAsync();

            Dictionary<GuildEmote, int> newEmoteCounts = await GetNewEmoteCounts(guildEmotes, textChannels, lastChannelMessages);



            //delete emotes counts for emoetes that no longer exist in the database
            HashSet<ulong> activeEmoteIds = guildEmotes.Select(e => e.Id).ToHashSet();

            bool hasStaleEmotes = await db.EmoteCounts.Where(e => e.GuildId == context.Guild.Id).AnyAsync(e => !activeEmoteIds.Contains(e.EmoteId));

            if (hasStaleEmotes)
            {
                await DeleteStaleEmoteCounts(activeEmoteIds, context);
            }
            //combine counts from db with new counts
            var combinedCounts = new Dictionary<GuildEmote, int>(newEmoteCounts);

            foreach (GuildEmote emote in guildEmotes)
            {
                newEmoteCounts.TryGetValue(emote, out int newCount);
                recordedEmoteCounts.TryGetValue(emote.Id, out int recordedCount);

                combinedCounts[emote] = newCount + recordedCount;
            }

            //update counts in db
            await UpdateOrAddEmoteCounts(combinedCounts, context);



            return combinedCounts;
        }


        public async Task<Dictionary<GuildEmote, int>> GetNewEmoteCounts(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<ITextChannel> textChannels, IEnumerable<LastChannelMessage> lastMessages)
        {
            var newEmoteCounts = new Dictionary<GuildEmote, int>();

            foreach (ITextChannel ch in textChannels)
            {
                Console.WriteLine(ch.Name);

                LastChannelMessage? lastChannelMessage = lastMessages.Where(m => m.ChannelId == ch.Id).FirstOrDefault();


                IMessage? lastMessage = lastChannelMessage != null ? await ch.GetMessageAsync(lastChannelMessage.MessageId) : null;

                while (true)
                {
                    List<IMessage> messages = (await (lastMessage == null ? ch.GetMessagesAsync(0, Direction.After, 100).FlattenAsync() : ch.GetMessagesAsync(lastMessage, Direction.After, 100).FlattenAsync())).ToList();

                    //Discord API returns messages in reverse chronological order even if using Direction.After so this reorders the list so the messages in the list are oldest to newest
                    messages.Reverse();

                    if (messages.Count == 0)
                    {
                        //Update last message in db
                        if (lastMessage != null)
                        {
                            await UpdateOrAddLastMessage(ch, lastMessage);
                        }
                        break;
                    }

                    lastMessage = messages[messages.Count - 1];

                    foreach (GuildEmote e in guildEmotes)
                    {
                        int count = messages.Count(m => (m.Content.Contains(e.ToString()) || m.Reactions.Any(reaction => reaction.Key.Equals(e))) && m.Author.Id != 1118358168708329543);

                        if (newEmoteCounts.ContainsKey(e))
                        {
                            newEmoteCounts[e] += count;
                        }
                        else
                        {
                            newEmoteCounts[e] = count;
                        }
                    }
                }
            }

            return newEmoteCounts;
        }
        public async Task UpdateOrAddLastMessage(ITextChannel channel, IMessage message)
        {
            using var db = new EmoteRankingDB();

            LastChannelMessage? existingEntry = await db.LastChannelMessages.FirstOrDefaultAsync(m => m.GuildId == channel.GuildId && m.ChannelId == channel.Id);

            if (existingEntry != null)
            {
                existingEntry.MessageId = message.Id;
                db.LastChannelMessages.Update(existingEntry);
            }
            else
            {
                LastChannelMessage newLastMessage = new LastChannelMessage
                {
                    GuildId = channel.GuildId,
                    ChannelId = channel.Id,
                    MessageId = message.Id
                };
                await db.LastChannelMessages.AddAsync(newLastMessage);

            }

            await db.SaveChangesAsync();
        }
        public async Task UpdateOrAddEmoteCounts(Dictionary<GuildEmote, int> emoteCounts, IInteractionContext context)
        {
            using var db = new EmoteRankingDB();

            DateTime dateTimeExecuted = DateTime.UtcNow;

            foreach (var (emote, count) in emoteCounts)
            {
                EmoteCount? existingEntry = await db.EmoteCounts.FirstOrDefaultAsync(e => e.EmoteId == emote.Id);

                if (existingEntry != null)
                {
                    existingEntry.Count = count;
                    existingEntry.DateTimeUpdated = dateTimeExecuted;
                    db.EmoteCounts.Update(existingEntry);
                }
                else
                {
                    EmoteCount newEmoteCount = new EmoteCount
                    {
                        Count = count,
                        DateTimeUpdated = dateTimeExecuted,
                        EmoteId = emote.Id,
                        GuildId = context.Guild.Id,
                    };
                    await db.EmoteCounts.AddAsync(newEmoteCount);
                }
            }

            await db.SaveChangesAsync();
        }
        public async Task DeleteStaleEmoteCounts(HashSet<ulong> activeEmoteIds, IInteractionContext context)
        {
            using var db = new EmoteRankingDB();

            var staleEmoteEntries = db.EmoteCounts.Where(e => e.GuildId == context.Guild.Id && !activeEmoteIds.Contains(e.EmoteId));

            db.EmoteCounts.RemoveRange(staleEmoteEntries);
            await db.SaveChangesAsync();
        }
        public async Task ResetDB(IInteractionContext context)
        {
            using var db = new EmoteRankingDB();

            var guildEmoteEntries = db.EmoteCounts.Where(e => e.GuildId == context.Guild.Id);
            var guildLastMessageEntries = db.LastChannelMessages.Where(e => e.GuildId == context.Guild.Id);

            db.EmoteCounts.RemoveRange(guildEmoteEntries);
            db.LastChannelMessages.RemoveRange(guildLastMessageEntries);
            await db.SaveChangesAsync();
        }
    }
}
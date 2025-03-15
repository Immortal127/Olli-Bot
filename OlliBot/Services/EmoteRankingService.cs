using Discord;
using Discord.WebSocket;

namespace OlliBot.Services
{
    public interface IEmoteRankingService
    {
        Task<Dictionary<GuildEmote, int>> CountEmoteUsage(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<SocketTextChannel> textChannels);
    }


    public class EmoteRankingService : IEmoteRankingService
    {
        public async Task<Dictionary<GuildEmote, int>> CountEmoteUsage(IReadOnlyCollection<GuildEmote> guildEmotes, IEnumerable<SocketTextChannel> textChannels)
        {
            var emoteCounts = new Dictionary<GuildEmote, int>();

            foreach (SocketTextChannel ch in textChannels)
            {
                Console.WriteLine(ch.Name);


                IMessage? lastMessage = null;

                while (true)
                {
                    List<IMessage> messages = (await (lastMessage == null ? ch.GetMessagesAsync(100).FlattenAsync() : ch.GetMessagesAsync(lastMessage, Direction.Before, 100).FlattenAsync())).ToList();

                    if (messages.Count == 0)
                    {
                        break;
                    }

                    lastMessage = messages[messages.Count - 1];

                    foreach (GuildEmote e in guildEmotes)
                    {
                        int count = messages.Count(m => (m.Content.Contains(e.ToString()) || m.Reactions.Any(reaction => reaction.Key.Equals(e))) && m.Author.Id != 1118358168708329543);

                        if (emoteCounts.ContainsKey(e))
                        {
                            emoteCounts[e] += count;
                        }
                        else
                        {
                            emoteCounts[e] = count;
                        }
                    }
                }
            }

            return emoteCounts;
        }
    }
}
        
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text;

namespace OlliBot.Modules
{
    public class Emotes : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("emoterank", "Emote rankings")]
        public async Task RankEmotes()
        {
            try
            {
                
                //Dictionary of emotes and an integer indicating number of uses 
                var emoteCounts = new Dictionary<GuildEmote, int>();

                //only emotes that are available
                IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;

                if (emotes.Count==0)
                {
                    await Context.Interaction.RespondAsync("No emotes found", ephemeral: true);
                    return;
                }

                //only text channels
                IEnumerable<SocketTextChannel> channelList = Context.Guild.Channels.OfType<SocketTextChannel>().Where(ch => ch.GetChannelType() == ChannelType.Text);

                await Context.Interaction.RespondAsync("Bot is working on counting emotes", ephemeral: true);

                foreach (SocketTextChannel ch in channelList)
                {
                    Console.WriteLine(ch.Name);


                    IMessage? lastMessage = null;

                    while (true)
                    {
                        List<IMessage> messages = (await (lastMessage == null ? ch.GetMessagesAsync(100).FlattenAsync() : ch.GetMessagesAsync(lastMessage, Direction.Before, 100).FlattenAsync())).ToList()  ;

                        if (messages.Count == 0)
                        {
                            break;
                        }

                        lastMessage = messages[messages.Count - 1];

                        foreach (GuildEmote e in emotes)
                        {
                            int count = messages.Count(m => (m.Content.Contains(e.ToString()) || m.Reactions.Any(reaction => reaction.Key.Equals(e))) && m.Author.Id!=1118358168708329543);

                            if (emoteCounts.ContainsKey(e))
                            {
                                emoteCounts[e]+=count;
                            }
                            else
                            {
                                emoteCounts[e]=count;
                            }
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Emote Usage Ranking:");

                foreach (KeyValuePair<GuildEmote, int> kv in emoteCounts.OrderByDescending(kv => kv.Value))
                {
                    sb.AppendLine($"{kv.Key}  -  {kv.Value}");
                }

                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
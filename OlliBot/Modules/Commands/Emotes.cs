using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Services;
using System.Text;

namespace OlliBot.Modules
{
    public class Emotes : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly IEmoteRankingService _emoteService;

        public Emotes(IEmoteRankingService emoteService)
        {
             _emoteService = emoteService;
        }

        [SlashCommand("emoterank", "Emote rankings")]
        public async Task RankEmotes()
        {
            try
            {
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

                Dictionary<GuildEmote, int> emoteCounts = await _emoteService.CountEmoteUsage(emotes, channelList);

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
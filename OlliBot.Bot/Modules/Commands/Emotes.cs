using Discord;
using Discord.Interactions;
using OlliBot.Bot.Interfaces;
using OlliBot.Bot.Utilities;

namespace OlliBot.Bot.Modules;

[RequireContext(ContextType.Guild)]
public class Emotes : InteractionModuleBase<SocketInteractionContext>
{

    private readonly IEmoteRankingService _emoteService;
    private readonly ILogger<Emotes> _logger;

    public Emotes(IEmoteRankingService emoteService, ILogger<Emotes> logger)
    {
        _emoteService = emoteService;
        _logger = logger;
    }

    [SlashCommand("emoterank", "Emote rankings")]
    public async Task SendEmoteRankings(
    [Choice("True", 1)]
    [Choice("False", 0)]
    [Summary("Reset")] int reset = 0)
    {
        try
        {
            //All custom emotes in a server
            IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;

            if (reset == 1)
            {
                await _emoteService.ResetDB(Context);
            }

            if (emotes.Count == 0)
            {
                await Context.Interaction.RespondAsync("No emotes found", ephemeral: true);
                return;
            }

            //All channels in a guild that can receive messages
            IEnumerable<ITextChannel> textChannels = Context.Guild.Channels.OfType<ITextChannel>();

            await Context.Interaction.RespondAsync("Bot is working on counting emotes", ephemeral: true);

            Dictionary<GuildEmote, int> emoteCounts = await _emoteService.GetEmoteCounts(emotes, textChannels, Context);




            string formattedRankings = Helpers.FormatEmoteRankings(emoteCounts);

            await Context.Channel.SendMessageAsync(formattedRankings);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}
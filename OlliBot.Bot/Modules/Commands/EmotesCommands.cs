using Discord;
using Discord.Interactions;
using OlliBot.Application.EmoteRanking.Commands;
using OlliBot.Bot.Interfaces;
using OlliBot.Bot.Utilities;

namespace OlliBot.Bot.Modules.Commands;

[RequireContext(ContextType.Guild)]
public class EmotesCommands(
    //IEmoteRankingService emoteService,
    UpdateEmoteRankingHandler updateHandler,
    ILogger<EmotesCommands> logger,
    UpdateEmoteRankingHandler clearHandler) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("emoterank", "Emote rankings")]
    public async Task SendEmoteRankings(
    [Choice("true", 1)]
    [Choice("false", 0)]
    [Summary("Reset", "Reset the emote ranking database.")] bool reset = false)
    {
        try
        {
            //All custom emotes in a server
            IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;

            if (reset == true)
            {
                UpdateEmoteRankingResult _ = await clearHandler.HandleAsync(new UpdateEmoteRankingCommand(Context.Guild.Id));
            }

            if (emotes.Count == 0)
            {
                await Context.Interaction.RespondAsync("No emotes found", ephemeral: true);
                return;
            }

            //All channels in a guild that can receive messages

            await Context.Interaction.RespondAsync("Bot is working on counting emotes", ephemeral: true);

            // call the update handler here
            var result = await updateHandler.HandleAsync(new UpdateEmoteRankingCommand(Context.Guild.Id));
            if (result.Counts == null || !result.Success)
            {
                await Context.Channel.SendMessageAsync(result.Message);
            }

            string formattedRankings = result.Counts != null ? Helpers.FormatEmoteRankings(result.Counts, emotes) : "No emote rankings available.";

            await Context.Channel.SendMessageAsync(formattedRankings);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An error occurred while handling emote ranking for {GuildId}",
                Context.Guild.Id);
        }
    }
}
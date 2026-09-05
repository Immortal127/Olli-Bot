using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using OlliBot.Application.EmoteRanking.ClearEmoteRanking;
using OlliBot.Application.EmoteRanking.UpdateEmoteRanking;
using OlliBot.Bot.Utilities;

namespace OlliBot.Bot.Modules.EmoteRanking;

[RequireContext(ContextType.Guild)]
[Group("emoterank", "Commands for emote ranking")]
public class EmoteRankingSlashCommands(
    ILogger<EmoteRankingSlashCommands> logger,
    ISender sender) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("scan", "Scan emote rankings")]
    public async Task UpdateAndDisplayEmoteRankingsAsync([Summary("Reset")] bool reset = false)
    {
        try
        {
            //All custom emotes in a server
            IReadOnlyCollection<GuildEmote> emotes = Context.Guild.Emotes;

            if (reset)
            {
                ClearEmoteRankingResult clearResult = await sender.Send(new ClearEmoteRankingCommand(Context.Guild.Id, ((SocketGuildUser)Context.User).GuildPermissions.Has(GuildPermission.Administrator)));
                if (!clearResult.Success)
                {
                    await Context.Interaction.RespondAsync(clearResult.Message, ephemeral: true);
                    return;
                }
            }

            if (emotes.Count == 0)
            {
                await Context.Interaction.RespondAsync("No emotes found", ephemeral: true);
                return;
            }

            //All channels in a guild that can receive messages
            await Context.Interaction.RespondAsync("Bot is working on counting emotes", ephemeral: true);

            // call the update handler here
            UpdateEmoteRankingResult result = await sender.Send(new UpdateEmoteRankingCommand(Context.Guild.Id));
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

    [SlashCommand("clear", "Clear emote rankings")]
    public async Task ClearEmoteRankingsAsync()
    {
        try
        {
            ClearEmoteRankingResult result = await sender.Send(new ClearEmoteRankingCommand(Context.Guild.Id, ((SocketGuildUser)Context.User).GuildPermissions.Has(GuildPermission.Administrator)));
            await Context.Interaction.RespondAsync(result.Message, ephemeral: true);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An error occurred while clearing emote ranking for {GuildId}",
                Context.Guild.Id);
            await Context.Interaction.RespondAsync("An error occurred while clearing emote rankings.", ephemeral: true);
        }
    }
}
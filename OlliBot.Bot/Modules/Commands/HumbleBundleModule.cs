using Discord;
using Discord.Interactions;

namespace OlliBot.Bot.Modules.Commands;

internal class HumbleBundleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("hb", "Humble Bundles")]
    public async Task GetHumbleBundles()
    {
        // Get humble bundles


        // Build embeds for each bundle 
        Embed embed = new EmbedBuilder()
            .Build();

        // Send found humbles to user / text channel
        await RespondAsync(embed: embed);
    }
}

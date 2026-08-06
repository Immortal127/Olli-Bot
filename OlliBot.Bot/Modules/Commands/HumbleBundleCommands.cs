using Discord;
using Discord.Interactions;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Modules.Commands;

[Group("hb", "humblebundle commands")]
public class HumbleBundleCommands(GetAllHumbleBundlesHandler getAllHandler) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("all", "Humble Bundles")]
    public async Task GetHumbleBundles(HumbleBundleType humbleBundleType)
    {
        // Get humble bundles
        var result = await getAllHandler.HandleAsync(new ScanHumbleBundleCommand(humbleBundleType));

        // Build embeds for each bundle 
        Embed embed = new EmbedBuilder()
            .Build();

        // Send found humbles to user / text channel
        foreach (var bundle in result.ScannedBundles)
        {
            await RespondAsync(embed: BuildHumbleBundleEmbed(bundle));
        }
    }

    private Embed BuildHumbleBundleEmbed(ScannedHumbleBundle bundle)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle(bundle.Name)
            .WithDescription($"Expires on: {bundle.ExpiryDate.ToShortDateString()}")
            .WithColor(Color.Blue);
        foreach (var tier in bundle.BundleTiers)
        {
            embedBuilder.AddField(tier.Price.ToString(), string.Join("\n", tier.HumbleBundleItems), inline: false);
        }
        return embedBuilder.Build();
    }
}

using Discord;
using Discord.Interactions;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;
using System.Text;

namespace OlliBot.Bot.Modules.Commands;

[Group("hb", "humblebundle commands")]
public class HumbleBundleCommands(GetAllHumbleBundlesHandler getAllHandler) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("all", "Humble Bundles")]
    public async Task GetHumbleBundles(HumbleBundleType humbleBundleType)
    {
        await RespondAsync("Retrieving Humble Bundles...", ephemeral: true);
        // Get humble bundles
        ScanHumbleBundleResult result = await getAllHandler.HandleAsync(new ScanHumbleBundleCommand(humbleBundleType));

        // Build embeds for each bundle 

        // Send found humbles to user / text channel
        foreach (ScannedHumbleBundle bundle in result.ScannedBundles)
        {
            await Context.Channel.SendMessageAsync(embed: CreateHumbleBundleEmbed(bundle));
        }
    }

    private Embed CreateHumbleBundleEmbed(ScannedHumbleBundle bundle)
    {
        string description = new StringBuilder()
            .Append($"**Expires:** {TimestampTag.FormatFromDateTime(bundle.ExpiryDate, TimestampTagStyles.ShortDateTime)} ({TimestampTag.FormatFromDateTime(bundle.ExpiryDate, TimestampTagStyles.Relative)})")
            .AppendLine()
            .AppendLine()
            .Append(bundle.ShortDescription)
            .AppendLine()
            .AppendLine()
            .Append($"**{bundle.Note}**")
            .ToString();

        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle(bundle.Name)
            .WithUrl(bundle.Url)
            .WithImageUrl(bundle.ImageUrl)
            .WithDescription(description)
            .WithColor(Color.Blue)
            .WithCurrentTimestamp();

        foreach (ScannedHumbleBundleTier tier in bundle.BundleTiers)
        {
            string items = string.Join(
                "\n",
                tier.HumbleBundleItems.Select(item =>
                    string.IsNullOrWhiteSpace(item.ExtraInfo)
                        ? $"• {item.ItemName}"
                        : $"• {item.ItemName} *({item.ExtraInfo})*"));

            embedBuilder.AddField(
                $"Tier {tier.Tier} - Pay at least {tier.Price:C}",
                items,
                inline: false);
        }

        return embedBuilder.Build();
    }
}

using Discord;
using OlliBot.Application.HumbleBundle.Models;
using System.Text;

namespace OlliBot.Bot.Modules.HumbleBundle;
internal static class HumbleBundleEmbedBuilder
{
    internal static Embed CreateHumbleBundleEmbed(ScannedHumbleBundle bundle)
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

        //$"• {item.ItemName}"

        foreach (ScannedHumbleBundleTier tier in bundle.BundleTiers)
        {
            string items = string.Join(
                "\n",
                tier.HumbleBundleItems.Select(item =>
                    string.IsNullOrWhiteSpace(item.ExtraInfo)
                        ? $"- {item.ItemName}"
                        : $"- {item.ItemName} *({item.ExtraInfo})*"));

            embedBuilder.AddField(
                $"Tier {tier.Tier} - Pay at least {tier.Price:C}",
                items,
                inline: false);
        }

        return embedBuilder.Build();
    }

}

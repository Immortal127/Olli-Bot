using Discord;
using OlliBot.Application.HumbleBundle.Models;
using System.Text;

namespace OlliBot.Bot.Modules.HumbleBundle;
internal static class HumbleBundleEmbedBuilder
{
    const string customBulletPoint = "•";
    const string discordBulletPoint = "-";

    static string bulletPoint => discordBulletPoint;

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

        foreach (ScannedHumbleBundleTier tier in bundle.BundleTiers.OrderBy(b => b.Tier))
        {
            string items = BuildEmbedFieldText(tier.HumbleBundleItems);


            embedBuilder.AddField(
                $"Tier {tier.Tier} - Pay at least {tier.Price:C}",
                items,
                inline: true);
        }

        return embedBuilder.Build();
    }

    private static string BuildEmbedFieldText(List<ScannedHumbleBundleItem> items)
    {
        const int maxLength = 1024;
        string moreText = $"{bulletPoint} And more...";

        var sb = new StringBuilder();

        for (int i = 0; i < items.Count; i++)
        {
            ScannedHumbleBundleItem item = items[i];

            string itemText = string.IsNullOrWhiteSpace(item.ExtraInfo)
                ? $"{bulletPoint} {item.ItemName}"
                : $"{bulletPoint} {item.ItemName} *({item.ExtraInfo})*";

            bool isLastItem = i == items.Count - 1;

            int requiredLength =
                itemText.Length +
                Environment.NewLine.Length;

            if (!isLastItem)
            {
                requiredLength += moreText.Length;
            }

            if (sb.Length + requiredLength > maxLength)
            {
                sb.Append(moreText);
                break;
            }

            sb.AppendLine(itemText);
        }

        return sb.ToString();
    }
}

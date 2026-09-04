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

    internal static MessageComponent CreateHumbleBundleEmbedV2(ScannedHumbleBundle scannedHumbleBundle)
    {
        var builder = new ComponentBuilderV2();

        var container = new ContainerBuilder();

        var buttons = new ActionRowBuilder()
            .WithButton(
                label: "Delete Notification",
                customId: $"delete_hb_notification:{scannedHumbleBundle.Url}",
                style: ButtonStyle.Danger
                //emote: new Emoji()
                )
            .WithButton(
                label: "View Bundle",
                //customId: $"view_hb_bundle:{scannedHumbleBundle.Url}",
                url: scannedHumbleBundle.Url,
                style: ButtonStyle.Link);


        container
            .WithTextDisplay(new TextDisplayBuilder()
            {
                Content = $"# **[{scannedHumbleBundle.Name}]({scannedHumbleBundle.Url})**"
            })
            .WithSeparator(spacing: SeparatorSpacingSize.Small)
            .WithTextDisplay(new TextDisplayBuilder()
            {
                Content = $"### **Expires:** {TimestampTag.FormatFromDateTime(scannedHumbleBundle.ExpiryDate, TimestampTagStyles.ShortDateTime)} ({TimestampTag.FormatFromDateTime(scannedHumbleBundle.ExpiryDate, TimestampTagStyles.Relative)})\n\n{scannedHumbleBundle.ShortDescription}\n\n**{scannedHumbleBundle.Note.Trim()}**",
            })
            .WithSeparator();

        //container.WithSection(new SectionBuilder().WithTextDisplay);

        foreach (var tier in scannedHumbleBundle.BundleTiers.OrderBy(b => b.Tier))
        {
            container
                .WithTextDisplay(new TextDisplayBuilder
                {
                    Content = $"Tier {tier.Tier} - Pay at least {tier.Price:C}"
                })
                .WithTextDisplay(new TextDisplayBuilder
                {
                    Content = BuildEmbedFieldText(tier.HumbleBundleItems)
                });

            //container.WithSection(tierSection);

            if (tier != scannedHumbleBundle.BundleTiers.OrderBy(b => b.Tier).Last())
            {
                container.WithSeparator();
            }
        }

        container
            .WithMediaGallery(new MediaGalleryBuilder().AddItem(new MediaGalleryItemProperties
            {
                Media = new UnfurledMediaItemProperties
                {
                    Url = scannedHumbleBundle.ImageUrl
                },
                //Description = "Bundle Thumbnail"
            }));

        container.WithActionRow(buttons);

        return builder.WithContainer(container).Build();
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

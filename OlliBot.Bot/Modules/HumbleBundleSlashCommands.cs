using Discord;
using Discord.Interactions;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;
using System.Text;


namespace OlliBot.Bot.Modules;

[Group("hb", "Humble bundle commands")]
public class HumbleBundleSlashCommands(
    GetAllHumbleBundlesHandler getAllHandler,
    AddHumbleBundleSubscriberHandler addSubscriberHandler) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("all", "Get all Humble Bundles of a specific type")]
    public async Task GetHumbleBundles([Summary("Type")] HumbleBundleType humbleBundleType)
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

    public async Task GetLatestHumbleBundle([Summary("Type")] HumbleBundleType humbleBundleType)
    {
        throw new NotImplementedException();
    }

    [SlashCommand("subscribe", "Subscribe for Humble Bundle updates")]
    public async Task SubscribeForHumbleBundleUpdates([Summary("Type")] HumbleBundleType humbleBundleType)
    {
        var command = new AddHumbleBundleSubscriberCommand(humbleBundleType, Context.User.Id, HumbleBundleSubscriberType.User);

        AddHumbleBundleSubscriberResult result = await addSubscriberHandler.HandleAsync(command);

        if (result.Success)
        {
            await RespondAsync($"Successfully subscribed to {humbleBundleType} Humble Bundle updates.", ephemeral: true);
        }
        else
        {
            await RespondAsync($"Failed to subscribe to {humbleBundleType} Humble Bundle updates: {result.Message}", ephemeral: true);
        }
    }

    public async Task ManageSubscriptions()
    {
        throw new NotImplementedException();
    }

    public async Task RemoveSubscription()
    {
        throw new NotImplementedException();
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
        {//•
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

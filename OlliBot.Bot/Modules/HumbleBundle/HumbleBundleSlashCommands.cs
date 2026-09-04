using Discord;
using Discord.Interactions;
using MediatR;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Modules.HumbleBundle;

[Group("hb", "Humble bundle commands")]
public class HumbleBundleSlashCommands(
    ISender sender) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("all", "Get all Humble Bundles of a specific type")]
    public async Task GetHumbleBundles([Summary("Type")] HumbleBundleType humbleBundleType)
    {
        await RespondAsync("Retrieving Humble Bundles...", ephemeral: true);
        // Get humble bundles
        ScanHumbleBundleResult result = await sender.Send(new ScanHumbleBundleCommand(humbleBundleType));

        // Build embeds for each bundle 
        // Send found humbles to user / text channel
        foreach (ScannedHumbleBundle bundle in result.ScannedBundles)
        {
            await Context.Channel.SendMessageAsync(embed: HumbleBundleEmbedBuilder.CreateHumbleBundleEmbed(bundle));
        }
    }

    [SlashCommand("latest", "Get the latest Humble Bundle of a specific type")]
    public async Task GetLatestHumbleBundle([Summary("Type")] HumbleBundleType humbleBundleType)
    {
        await RespondAsync("Retrieving latest Humble Bundle...", ephemeral: true);

        // Get humble bundles
        ScanHumbleBundleResult result = await sender.Send(new ScanHumbleBundleCommand(humbleBundleType));

        await Context.Channel.SendMessageAsync(components: HumbleBundleEmbedBuilder.CreateHumbleBundleEmbedV2(result.ScannedBundles.First()));
    }

    [SlashCommand("subscribe", "Subscribe for Humble Bundle updates")]
    public async Task ManageSubscriptions()
    {
        var subscriptions = await sender.Send(new GetUserHumbleBundleSubscriptionsQuery(Context.User.Id));

        var subscriptionList = subscriptions.HumbleBundleTypes;

        var selectMenu = new SelectMenuBuilder()
            .WithCustomId("bundle_types")
            .WithMinValues(0)
            .WithMaxValues(3)
            .AddOption(
                HumbleBundleType.Games.ToString(),
                "games",
                isDefault: subscriptionList.Contains(HumbleBundleType.Games))
            .AddOption(
                HumbleBundleType.Software.ToString(),
                "software",
                isDefault: subscriptionList.Contains(HumbleBundleType.Software))
            .AddOption(
                HumbleBundleType.Books.ToString(),
                "books",
                isDefault: subscriptionList.Contains(HumbleBundleType.Books));

        var components = new ComponentBuilderV2()
            .WithTextDisplay("### Select bundle types to subscribe to")
            .WithActionRow(
                new ActionRowBuilder()
                    .WithSelectMenu(selectMenu))
            .Build();

        await RespondAsync(
            components: components,
            flags: MessageFlags.Ephemeral | MessageFlags.ComponentsV2);
    }

    [ComponentInteraction("bundle_types", ignoreGroupNames: true)]
    public async Task UpdateBundleSubscriptionsAsync(string[] bundleTypesString)
    {
        HumbleBundleType[] selectedBundleTypes = bundleTypesString
            .Select(x => Enum.Parse<HumbleBundleType>(x, ignoreCase: true))
            .ToArray();

        var currentSubscriptions = await sender.Send(
            new GetUserHumbleBundleSubscriptionsQuery(Context.User.Id));

        HumbleBundleType[] subscriptionsToAdd = selectedBundleTypes
            .Except(currentSubscriptions.HumbleBundleTypes)
            .ToArray();

        HumbleBundleType[] subscriptionsToRemove = currentSubscriptions.HumbleBundleTypes
            .Except(selectedBundleTypes)
            .ToArray();

        var messages = new List<string>();

        foreach (var bundleType in subscriptionsToAdd)
        {
            var command = new AddHumbleBundleSubscriberCommand(
                bundleType,
                Context.User.Id,
                HumbleBundleSubscriberType.User);

            AddHumbleBundleSubscriberResult result = await sender.Send(command);

            if (result.Success)
            {
                messages.Add($"Subscribed to {bundleType} Humble Bundle updates.");
            }
            else
            {
                messages.Add(
                    $"Failed to subscribe to {bundleType}: {result.Message}");
            }
        }

        foreach (var bundleType in subscriptionsToRemove)
        {
            var command = new RemoveHumbleBundleSubscriberCommand(
                bundleType,
                Context.User.Id,
                HumbleBundleSubscriberType.User);

            RemoveHumbleBundleSubscriberResult result = await sender.Send(command);

            if (result.Success)
            {
                messages.Add($"Unsubscribed from {bundleType} Humble Bundle updates.");
            }
            else
            {
                messages.Add(
                    $"Failed to unsubscribe from {bundleType}: {result.Message}");
            }
        }

        await RespondAsync(
            string.Join(Environment.NewLine, messages),
            ephemeral: true);
    }

    //[SlashCommand("scan-silently", "Silently scan for Humble Bundle updates")]
    //public async Task SilentlyScanHumbleBundles()
    //{
    //    await RespondAsync("Retrieving Humble Bundles...", ephemeral: true);
    //    // Get humble bundles
    //    CheckForHumbleBundleUpdatesResult result = await checkHandler.HandleAsync(new CheckForHumbleBundleUpdatesCommand(HumbleBundleType.Games));
    //}

    //public async Task RemoveSubscription()
    //{
    //    throw new NotImplementedException();
    //}
}

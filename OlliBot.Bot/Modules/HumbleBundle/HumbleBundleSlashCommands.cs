using Discord.Interactions;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;


namespace OlliBot.Bot.Modules.HumbleBundle;

[Group("hb", "Humble bundle commands")]
public class HumbleBundleSlashCommands(
    GetAllHumbleBundlesHandler getAllHandler,
    AddHumbleBundleSubscriberHandler addSubscriberHandler,
    CheckForHumbleBundleUpdatesHandler checkHandler) : InteractionModuleBase<SocketInteractionContext>
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
            await Context.Channel.SendMessageAsync(embed: HumbleBundleEmbedBuilder.CreateHumbleBundleEmbed(bundle));
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

    //[SlashCommand("scan-silently", "Silently scan for Humble Bundle updates")]
    //public async Task SilentlyScanHumbleBundles()
    //{
    //    await RespondAsync("Retrieving Humble Bundles...", ephemeral: true);
    //    // Get humble bundles
    //    CheckForHumbleBundleUpdatesResult result = await checkHandler.HandleAsync(new CheckForHumbleBundleUpdatesCommand(HumbleBundleType.Games));
    //}

    public async Task ManageSubscriptions()
    {
        throw new NotImplementedException();
    }

    public async Task RemoveSubscription()
    {
        throw new NotImplementedException();
    }
}

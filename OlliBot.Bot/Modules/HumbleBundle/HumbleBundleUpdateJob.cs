using Discord;
using MediatR;
using OlliBot.Application.HumbleBundle.CheckForHumbleBundleUpdates;
using Quartz;
using System.Data;

namespace OlliBot.Bot.Modules.HumbleBundle;

[DisallowConcurrentExecution]
internal class HumbleBundleUpdateJob(
    ISender sender,
    ILogger<HumbleBundleUpdateJob> logger,
    IDiscordClient discordClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken ct = context.CancellationToken;

        logger.LogInformation("Scheduled Humble Bundle update started.");
        foreach (Domain.Enums.HumbleBundleType bundleType in Enum.GetValues<Domain.Enums.HumbleBundleType>())
        {

            try
            {
                var command = new CheckForHumbleBundleUpdatesCommand(bundleType);

                CheckForHumbleBundleUpdatesResult result = await sender.Send(command, ct);
                if (!result.Success)
                {
                    logger.LogError(
                        "Scheduled Humble Bundle update failed: {Message}",
                        result.Message);
                }

                if (!result.ScannedBundles.Any())
                {
                    logger.LogInformation("Scheduled Humble Bundle update found no new bundles of type {type}", bundleType);
                    continue;
                }

                IEnumerable<Domain.Entities.HumbleBundleSubscriber> userSubscribers = result.Subscribers.Where(s => s.SubscriberType == Domain.Enums.HumbleBundleSubscriberType.User);
                IEnumerable<Domain.Entities.HumbleBundleSubscriber> channelSubscribers = result.Subscribers.Where(s => s.SubscriberType == Domain.Enums.HumbleBundleSubscriberType.Channel);

                foreach (Application.HumbleBundle.Models.ScannedHumbleBundle bundle in result.ScannedBundles)
                {
                    MessageComponent component = HumbleBundleEmbedBuilder.CreateHumbleBundleComponentV2(bundle);

                    foreach (Domain.Entities.HumbleBundleSubscriber? subscriber in userSubscribers)
                    {
                        IUser discordUser = await discordClient.GetUserAsync(subscriber.DiscordId);

                        await discordUser.SendMessageAsync(components: component);
                    }

                    foreach (Domain.Entities.HumbleBundleSubscriber? subscriber in channelSubscribers)
                    {
                        if (subscriber.GuildId is null)
                        {
                            logger.LogWarning(
                                "Discord channel subscriber {SubscriberId} has no guild ID.",
                                subscriber.Id);
                            continue;
                        }

                        IGuild guild = await discordClient.GetGuildAsync(subscriber.GuildId.Value, CacheMode.AllowDownload, new RequestOptions
                        {
                            CancelToken = ct
                        });

                        IChannel? channel = await guild.GetChannelAsync(
                            subscriber.DiscordId,
                            CacheMode.AllowDownload,
                            new RequestOptions
                            {
                                CancelToken = ct
                            });

                        string roleMention = string.Empty;
                        if (subscriber.RoleId.HasValue)
                        {
                            IRole role = guild.GetRole(subscriber.RoleId.Value);
                            roleMention = $"\n\n<@&{role.Id}>";

                            // save this for later, maybe worth considering in future

                            //if (role is not null)
                            //{
                            //    embed.Description += $"\n\n<@&{role.Id}>"; // Mention the role in the embed description
                            //}
                        }

                        if (channel is not IMessageChannel messageChannel)
                        {
                            logger.LogWarning(
                                "Discord channel {ChannelId} was not found or cannot receive messages.",
                                subscriber.DiscordId);

                            return;
                        }

                        await messageChannel.SendMessageAsync(components: component, text: roleMention);
                    }
                }
            }
            catch (OperationCanceledException)
                when (ct.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Scheduled Humble Bundle update was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Scheduled Humble Bundle update failed.");

                throw new JobExecutionException(ex);
            }
        }
    }
}

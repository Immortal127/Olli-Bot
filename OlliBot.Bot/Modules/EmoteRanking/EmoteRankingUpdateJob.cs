using Discord;
using MediatR;
using OlliBot.Application.EmoteRanking.UpdateEmoteRanking;
using Quartz;

namespace OlliBot.Bot.Modules.EmoteRanking;
[DisallowConcurrentExecution]
internal class EmoteRankingUpdateJob(
    ILogger<EmoteRankingUpdateJob> logger,
    ISender sender,
    IDiscordClient discordClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Starting emote ranking update job.");
        foreach (IGuild? guild in await discordClient.GetGuildsAsync())
        {
            if (guild.Emotes.Count == 0)
            {
                logger.LogWarning("Guild {GuildId} has no emotes, skipping emote ranking update.", guild.Id);
                return;
            }

            await sender.Send(new UpdateEmoteRankingCommand(guild.Id), context.CancellationToken);
        }
        logger.LogInformation("Emote ranking update job completed successfully.");
    }
}

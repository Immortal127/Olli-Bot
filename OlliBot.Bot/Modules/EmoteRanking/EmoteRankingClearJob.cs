using Discord;
using MediatR;
using OlliBot.Application.EmoteRanking.ClearEmoteRanking;
using Quartz;

namespace OlliBot.Bot.Modules.EmoteRanking;

[DisallowConcurrentExecution]
internal class EmoteRankingClearJob(
    ILogger<EmoteRankingClearJob> logger,
    ISender sender,
    IDiscordClient discordClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Starting emote ranking clear job");
        foreach (IGuild? guild in await discordClient.GetGuildsAsync())
        {
            logger.LogInformation(
                "Clearing emote ranking for guild {GuildId} ({GuildName})",
                guild.Id,
                guild.Name);

            await sender.Send(new ClearEmoteRankingCommand(guild.Id, true), context.CancellationToken);
        }
    }
}

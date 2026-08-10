using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Application.EmoteRanking.Scanning;
using OlliBot.Bot.Mappers;
using OlliBot.Bot.Modules;
using OlliBot.Bot.Modules.HumbleBundle;
using OlliBot.Bot.Services;
using Quartz;
using Quartz.Util;
using Serilog;

namespace OlliBot.Bot;
internal static class DependencyInjection
{
    internal static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        services.AddTransient<AddMessageCommandMapper>();
        services.AddHostedService<BotHostedService>();

        services.AddSingleton<DiscordEventListener>();

        services.AddTransient<IEmoteCountScanner, EmoteCountScanner>();
        return services;
    }

    internal static IServiceCollection AddDiscordServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<DiscordLogService>();

        services.AddSingleton<DiscordSocketClient>((serviceProvider) =>
        {
            try
            {
                var discordClient = new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 5000,
                    AlwaysDownloadUsers = true,
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.GuildMembers,
                    //GatewayIntents = GatewayIntents.All
                });

                //DiscordLogService discordLogService = serviceProvider.GetRequiredService<DiscordLogService>();
                //discordClient.MessageReceived +=

                return discordClient;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize Discord Socket Client");
                throw;
            }
        });

        services.AddSingleton((serviceProvider) =>
        {
            DiscordSocketClient discordClient = serviceProvider.GetRequiredService<DiscordSocketClient>();

            var interaction = new InteractionService(discordClient.Rest);

            return interaction;
        });

        services.AddSingleton<IDiscordClient>(sp => sp.GetRequiredService<DiscordSocketClient>());

        return services;
    }

    internal static IServiceCollection AddScheduling(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        const string groupName = "humble-bundle";

        string cronExpression =
            configuration["Scheduling:HumbleBundleUpdate:Cron"]
            ?? throw new InvalidOperationException(
                "The Humble Bundle cron schedule was not configured.");

        if (!CronExpression.IsValidExpression(cronExpression))
        {
            throw new InvalidOperationException(
                $"'{cronExpression}' is not a valid Quartz cron expression.");
        }

        string timeZoneId =
            configuration["Scheduling:HumbleBundleUpdate:TimeZone"]
            ?? "Europe/London";

        TimeZoneInfo timeZone =
            TimeZoneUtil.FindTimeZoneById(timeZoneId);

        var jobKey = new JobKey(
            "check-for-humble-bundle-updates",
            groupName);

        // Explicit registration validates its dependencies at startup.
        services.AddScoped<HumbleBundleUpdateJob>();

        services.AddQuartz(quartz =>
        {
            quartz.SchedulerName = "OlliBot";
            quartz.UseInMemoryStore();

            quartz.AddJob<HumbleBundleUpdateJob>(job =>
                job
                    .WithIdentity(jobKey)
                    .WithDescription(
                        "Checks for new Humble Bundles."));

            quartz.AddTrigger(trigger =>
                trigger
                    .WithIdentity(
                        "daily-humble-bundle-update",
                        groupName)
                    .ForJob(jobKey)
                    .WithCronSchedule(
                        cronExpression,
                        schedule => schedule
                            .InTimeZone(timeZone)
                            .WithMisfireHandlingInstructionFireAndProceed()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    internal static IServiceCollection ConfigureWindowService(this IServiceCollection services)
    {
        services.AddWindowsService(options =>
        {
            options.ServiceName = "OlliBot";
        });

        return services;
    }
}

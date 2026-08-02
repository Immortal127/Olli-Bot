using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Bot.Interfaces;
using OlliBot.Bot.Modules;
using OlliBot.Bot.Services;
using Serilog;

namespace OlliBot.Bot;
internal static class DependencyInjection
{
    internal static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        services.AddTransient<IMessageFactory, MessageFactory>();
        services.AddScoped<IEmoteRankingService, EmoteRankingService>();
        services.AddTransient<AddMessageCommandMapper>();

        services.AddTransient<BotInitialization>();
        services.AddSingleton<InteractionHandler>();
        services.AddSingleton<BotEventHandler>();
        services.AddHostedService<BotHostedService>();
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

                DiscordLogService discordLogService = serviceProvider.GetRequiredService<DiscordLogService>();
                discordClient.Log += discordLogService.Log;

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
}

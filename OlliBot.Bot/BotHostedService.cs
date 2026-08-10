using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Bot.Services;

namespace OlliBot.Bot;

public sealed class BotHostedService(
    ILogger<BotHostedService> logger,
    DiscordSocketClient client,
    InteractionService interaction,
    IConfiguration configuration,
    DiscordLogService discordLogService,
    DiscordEventListener discordEventListener) : BackgroundService
{
    private static readonly string ProjectName = typeof(BotHostedService).Assembly.GetName().Name!;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Project} starting...", ProjectName);

        SubscribeToDiscordEvents();

        logger.LogInformation("Owner ID: {OwnerID}", configuration["OwnerID"] ?? "Owner ID not configured");

        await base.StartAsync(cancellationToken);
    }

    private void UnsubscribeFromDiscordEvents()
    {
        client.Log -= discordLogService.Log;
        client.Ready -= discordEventListener.OnClientReady;

        client.InteractionCreated -= discordEventListener.OnInteractionCreated;
        //client.InteractionCreated += discordEventListener.OnSlashInvoked;

        client.MessageReceived -= discordEventListener.OnMessageReceivedAsync;
        interaction.SlashCommandExecuted -= discordEventListener.OnSlashCommandExecuted;
    }

    private void SubscribeToDiscordEvents()
    {
        client.Log += discordLogService.Log;
        client.Ready += discordEventListener.OnClientReady;

        client.InteractionCreated += discordEventListener.OnInteractionCreated;
        //client.InteractionCreated += interactionHandler.OnSlashInvoked;

        client.MessageReceived += discordEventListener.OnMessageReceivedAsync;
        interaction.SlashCommandExecuted += discordEventListener.OnSlashCommandExecuted;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Project} disconnecting...", ProjectName);

        try
        {
            await client.StopAsync();
            await client.LogoutAsync();
            logger.LogInformation("{Project} disconnected...", ProjectName);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error occurred while shutting down");
            throw;
        }
        finally
        {
            UnsubscribeFromDiscordEvents();

            await base.StopAsync(cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            string token = configuration["Discord:BotToken"] ?? throw new InvalidOperationException("DiscordBotToken is not configured");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            logger.LogInformation("{Project} running...", ProjectName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Expected during shutdown.
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Bot failed");
            throw;
        }
    }
}

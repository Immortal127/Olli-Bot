using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Bot.Modules;

namespace OlliBot.Bot;

public class BotHostedService(
    ILogger<BotHostedService> logger,
    DiscordSocketClient client,
    InteractionService interaction,
    IConfiguration configuration,
    BotInitialization botInitialization,
    InteractionHandler interactionHandler,
    BotEventHandler eventHandler) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OlliBot.Bot starting...");

        client.Ready += botInitialization.InitializationTasks;

        // TODO: Consider subscribing to other events
        client.InteractionCreated += interactionHandler.HandleInteraction;
        client.InteractionCreated += interactionHandler.OnSlashInvoked;

        client.MessageReceived += eventHandler.OnMessage;
        interaction.SlashCommandExecuted += eventHandler.OnSlashExecute;

        logger.LogInformation(configuration["OwnerID"] ?? "Owner ID not configured");

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OlliBot.Bot disconnecting...");

        try
        {
            await client.StopAsync();
            await client.LogoutAsync();
            logger.LogInformation("OlliBot.Bot disconnected...");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error occured while shutting down");
            throw;
        }

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            string token = configuration["DiscordBotToken"] ?? throw new InvalidOperationException("DiscordBotToken is not configured");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            logger.LogInformation("OlliBot.Bot running...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown Ctrl+C
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Bot failed");
            throw;
        }
    }
}

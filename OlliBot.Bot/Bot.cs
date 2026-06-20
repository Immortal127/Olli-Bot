using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Bot.Modules;

namespace OlliBot.Bot;

public class Bot(
    ILogger<Bot> logger,
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

        client.InteractionCreated += interactionHandler.HandleInteraction;
        client.InteractionCreated += interactionHandler.OnSlashInvoked;

        client.MessageReceived += eventHandler.OnMessage;

        interaction.SlashCommandExecuted += eventHandler.OnSlashExecute;

        logger.LogInformation(configuration["OwnerID"] ?? "Owner ID not configured");

        try
        {
            await client.LoginAsync(TokenType.Bot, configuration["DiscordBotToken"]);
            //await _client.LoginAsync(TokenType.Bot, _configuration["DiscordBotToken"]);
            await client.StartAsync();
        }
        catch (Exception ex)
        {

            logger.LogCritical($"Client failed to connect: {ex.Message}");

            throw;
        }
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OlliBot.Bot disconnecting...");

        client.Ready -= botInitialization.InitializationTasks;
        client.InteractionCreated -= interactionHandler.HandleInteraction;
        client.InteractionCreated -= interactionHandler.OnSlashInvoked;
        client.MessageReceived -= eventHandler.OnMessage;
        interaction.SlashCommandExecuted -= eventHandler.OnSlashExecute;

        try
        {
            await client.StopAsync();
            await client.LogoutAsync();
            logger.LogInformation("OlliBot.Bot disconnected...");
        }
        catch (Exception ex)
        {
            logger.LogCritical($"Error occured while shutting down: {ex.Message}");
            throw;
        }

        interaction.Dispose();
        client.Dispose();

        await base.StopAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("OlliBot.Bot running...");

        // Keep the service running until a cancellation is requested.
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown (Ctrl+C) – ignore
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during shutdown");
        }
    }
}

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace OlliBot.Host;

public class Bot : BackgroundService
{

    private readonly ILogger<Bot> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interaction;
    private readonly IConfiguration _configuration;
    private readonly BotInitialization _botInitialization;
    private readonly InteractionHandler _interactionHandler;
    private readonly BotEventHandler _eventHandler;

    public Bot(ILogger<Bot> logger, DiscordSocketClient client, InteractionService interaction, IConfiguration configuration, BotInitialization botInitialization, InteractionHandler interactionHandler, BotEventHandler eventHandler)
    {
        _logger = logger;
        _client = client;
        _interaction = interaction;
        _configuration = configuration;
        _botInitialization = botInitialization;
        _interactionHandler = interactionHandler;
        _eventHandler = eventHandler;

    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OlliBot.Bot starting...");
        _client.Ready += _botInitialization.InitializationTasks;

        _client.InteractionCreated += _interactionHandler.HandleInteraction;
        _client.InteractionCreated += _interactionHandler.OnSlashInvoked;

        _client.MessageReceived += _eventHandler.OnMessage;

        _interaction.SlashCommandExecuted += _eventHandler.OnSlashExecute;

        _logger.LogInformation(_configuration["OwnerID"] ?? "Owner ID not configured");

        try
        {
            await _client.LoginAsync(TokenType.Bot, _configuration["DiscordBotToken"]);
            //await _client.LoginAsync(TokenType.Bot, _configuration["DiscordBotToken"]);
            await _client.StartAsync();
        }
        catch (Exception ex)
        {

            _logger.LogCritical($"Client failed to connect: {ex.Message}");

            throw;
        }
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OlliBot.Bot disconnecting...");

        _client.Ready -= _botInitialization.InitializationTasks;
        _client.InteractionCreated -= _interactionHandler.HandleInteraction;
        _client.InteractionCreated -= _interactionHandler.OnSlashInvoked;
        _client.MessageReceived -= _eventHandler.OnMessage;
        _interaction.SlashCommandExecuted -= _eventHandler.OnSlashExecute;

        try
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
            _logger.LogInformation("OlliBot.Bot disconnected...");
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error occured while shutting down: {ex.Message}");
            throw;
        }

        _interaction.Dispose();
        _client.Dispose();

        await base.StopAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OlliBot.Bot running...");

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
            _logger.LogError(ex, "An error occurred during shutdown");
        }
    }
}

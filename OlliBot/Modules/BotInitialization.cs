using Discord;
using Discord.Interactions;
using System.Reflection;

namespace OlliBot.Modules;

public class BotInitialization
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Bot> _logger;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDiscordClient _client;
    public BotInitialization(IConfiguration configuration, ILogger<Bot> logger, InteractionService interaction, IServiceProvider serviceProvider, IDiscordClient client)
    {
        _configuration = configuration;
        _logger = logger;
        _interaction = interaction;
        _serviceProvider = serviceProvider;
        _client = client;
    }

    public async Task InitializationTasks()
    {
        _logger.LogInformation("Initialization tasks running...");
        _logger.LogInformation($"Bot ID: {_configuration["BotID"] ?? _client.CurrentUser.Id.ToString()}");

        _logger.LogInformation("Registering commands...");
        await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await _interaction.RegisterCommandsGloballyAsync();


        foreach (SlashCommandInfo slashCommand in _interaction.SlashCommands.ToList())
        {
            _logger.LogInformation($"Registered {slashCommand}");
        }

        await Task.CompletedTask;
    }
}
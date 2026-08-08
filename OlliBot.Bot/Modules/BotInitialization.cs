using Discord;
using Discord.Interactions;
using System.Reflection;

namespace OlliBot.Bot.Modules;

public class BotInitialization(
    ILogger<BotHostedService> logger,
    InteractionService interaction,
    IServiceProvider serviceProvider,
    IDiscordClient client)
{
    public async Task InitializationTasks()
    {
        logger.LogInformation("Initialization tasks running...");
        logger.LogInformation("Bot ID: {BotId}", client.CurrentUser.Id);

        logger.LogInformation("Registering commands...");
        await interaction.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        await interaction.RegisterCommandsGloballyAsync();

        foreach (SlashCommandInfo slashCommand in interaction.SlashCommands.ToList())
        {
            logger.LogInformation("Registered {SlashCommand}...", slashCommand.Name);
        }

        await Task.CompletedTask;
    }
}
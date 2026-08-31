using Discord;
using Discord.Interactions;
using MediatR;
using System.Reflection;

namespace OlliBot.Bot.Notifications.Handlers;
internal class ClientReadyHandler(
    ILogger<ClientReadyNotification> logger,
    InteractionService interaction,
    IServiceProvider serviceProvider,
    IDiscordClient client) : INotificationHandler<ClientReadyNotification>
{
    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Initialization tasks running...");
        logger.LogInformation("Bot ID: {BotId}", client.CurrentUser.Id);

        logger.LogInformation("Registering commands...");
        await interaction.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        await interaction.RegisterCommandsGloballyAsync();

        foreach (SlashCommandInfo slashCommand in interaction.SlashCommands.ToList())
        {
            logger.LogInformation("Registered Slash Command: {SlashCommand}", slashCommand.Name);
        }

        foreach (ContextCommandInfo contextCommand in interaction.ContextCommands.ToList())
        {
            logger.LogInformation("Registered Context Command: {ContextCommand}", contextCommand.Name);
        }

        foreach (ModalCommandInfo modalCommand in interaction.ModalCommands)
        {
            logger.LogInformation(
                "Registered Modal Command: {ModalCommand}",
                modalCommand.Name);
        }

        await Task.CompletedTask;
    }
}

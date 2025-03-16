using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text;

namespace OlliBot.Modules
{
    public class InteractionHandler
    {
        private readonly InteractionService _interactionService;
        private readonly ILogger<Bot> _logger;
        private readonly IDiscordClient _client;
        private readonly IServiceProvider _serviceProvider;

        public InteractionHandler(InteractionService interactionService, ILogger<Bot> logger, IDiscordClient client, IServiceProvider serviceProvider)
        {
            _interactionService = interactionService;
            _logger = logger;
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error handling interaction.");
            }
        }
        public Task OnSlashInvoked(SocketInteraction interaction)
        {
            var command = interaction as SocketSlashCommand;

            StringBuilder logMessage = new StringBuilder();

            if (command is null)
            {
                return Task.CompletedTask;
            }

            logMessage.Append($"Command invoked: {command.CommandName} ");

            if (command.Data.Options.Count!=0)
            {
                logMessage.Append("(");
                foreach (var option in command.Data.Options.Where(option => option != null))
                {
                    logMessage.Append($"{option.Name}:{option.Value}, ");
                }
                if (logMessage[logMessage.Length - 2] == ',') // Removing the trailing comma and space
                {
                    logMessage.Length -= 2;
                }
                logMessage.Append(") ");
            }
            logMessage.Append($"by {command.User.Username}, {command.User.Id}");
            _logger.LogInformation($"{logMessage}");
            return Task.CompletedTask;
        }
    }
}

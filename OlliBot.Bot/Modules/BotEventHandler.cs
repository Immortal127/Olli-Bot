using Discord;
using Discord.Interactions;
using OlliBot.Bot.Utilities;

namespace OlliBot.Bot.Modules;

public class BotEventHandler(IConfiguration configuration, IDiscordClient client, ILogger<BotEventHandler> logger)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IDiscordClient _client = client;
    private readonly ILogger<BotEventHandler> _logger = logger;

    public async Task OnMessage(IMessage message)
    {
        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            return;
        }

        var channel = (ITextChannel)message.Channel;
        IGuild guild = channel.Guild;

        if (!message.IsAuthorOlliBot(_client) && message.Content.Contains("good bot", StringComparison.OrdinalIgnoreCase) && (await channel.GetMessagesAsync(message.Id, Direction.Before, 10).FlattenAsync()).Any(m => m.IsAuthorOlliBot(_client)))
        {
            await channel.SendMessageAsync(":3", messageReference: new MessageReference(message.Id));
        }
        if (message.ContentExeedsLength(150) && !message.IsAuthorOlliBot(_client))
        {
            await channel.SendMessageAsync("i ain't reading all that", messageReference: new MessageReference(message.Id));
            await channel.SendMessageAsync("i'm happy for u tho");
            await channel.SendMessageAsync("or sorry that happened");
            return;
        }

        if (message.Author.Id == 164740251934392321 && guild.Id.ToString() == _configuration["MainServer"] && new Random().Next(1, 101) <= 15)
        {
            await channel.SendMessageAsync("James Here", messageReference: new MessageReference(message.Id));
        }
    }
    public async Task OnSlashExecute(SlashCommandInfo slashInfo, IInteractionContext ctx, IResult result)
    {
        if (result.IsSuccess)
        {
            _logger.LogInformation("Command '{Command}' completed without InteractionService errors", slashInfo.Name);
        }
        else if (!result.IsSuccess)
        {
            if (result.Error == InteractionCommandError.UnmetPrecondition)
            {
                if (result.ErrorReason == "Invalid context for command; accepted contexts: Guild.")
                {
                    await ctx.Interaction.RespondAsync("Command can only be used in a server.", ephemeral: true);
                    _logger.LogWarning($"{result.ErrorReason}");
                }
                if (result.Error == InteractionCommandError.UnmetPrecondition)
                {
                    await ctx.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
                    _logger.LogWarning($"{result.ErrorReason}");
                }
            }
        }
    }
}
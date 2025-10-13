using Discord;
using Discord.Interactions;
using OlliBot.Utilities;

namespace OlliBot.Modules;

public class BotEventHandler
{
    private readonly IConfiguration _configuration;
    private readonly IDiscordClient _client;
    private readonly ILogger<Bot> _logger;

    public BotEventHandler(IConfiguration configuration, IDiscordClient client, ILogger<Bot> logger)
    {
        _configuration = configuration;
        _client = client;
        _logger = logger;
    }

    public async Task OnMessage(IMessage message)
    {
        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            return;
        }

        var channel = (ITextChannel)message.Channel;
        IGuild guild = channel.Guild;

        if (message.ContentExeedsLength(150) && !message.IsAuthorOlliBot(_client))
        {
            await message.Channel.SendMessageAsync("i ain't reading all that", messageReference: new MessageReference(message.Id));
            await message.Channel.SendMessageAsync("i'm happy for u tho");
            await message.Channel.SendMessageAsync("or sorry that happened");
            return;
        }

        if (message.Author.Id == 164740251934392321 && guild.Id.ToString() == _configuration["MainServer"] && new Random().Next(1, 101) <= 15)
        {
            await message.Channel.SendMessageAsync("James Here", messageReference: new MessageReference(message.Id));
        }
    }
    public async Task OnSlashExecute(SlashCommandInfo slashInfo, IInteractionContext ctx, IResult result)
    {
        if (result.IsSuccess)
        {
            _logger.LogInformation("Command executed successfully");
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
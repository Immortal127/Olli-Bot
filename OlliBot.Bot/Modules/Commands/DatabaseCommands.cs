using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Application.Messages.Commands;
using OlliBot.Application.Messages.Queries;
using OlliBot.Bot.Mappers;
using OlliBot.Domain.Entities;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Modules.Commands;

[RequireContext(ContextType.Guild)]
[Group("db", "Commands to interact with the message database")]
public class DatabaseCommands(
    ILogger<DatabaseCommands> logger,
    AddMessageHandler addMessageHandler,
    AddMessageCommandMapper addMessageMapper,
    CallMessageHandler callMessageHandler,
    DeleteMessageHandler deleteMessageHandler,
    ListMessageHandler listMessageHandler,
    UpdateMessageHandler updateMessageHandler) : InteractionModuleBase<SocketInteractionContext>
{

    //Command to add entries to database
    [SlashCommand("add", "Add a discord message to the database")]
    public async Task AddMessage([Summary("message", "Enter a message ID or quote content")] string messageEntry,
    [Summary("title", "Title (Optional)")] string? title = null,
    [Summary("origin", "Quote origin (Optional if using Message ID for input)")] SocketGuildUser? originUser = null,
    [Choice("Meme", "Meme")]
    [Choice("Quote", "Quote")]
    [Choice("Other", "Other")]
    [Summary("Type", "Type (If no value set then will be implicitly determined)")] string? messageTypeString = null)
    {
        AddMessageCommand addCommand;

        if (ulong.TryParse(messageEntry, out ulong result))
        {
            IMessage discordMessage = await Context.Interaction.Channel.GetMessageAsync(result);
            addCommand = addMessageMapper.Map(discordMessage, title, Context, messageTypeString);
        }
        //If input for message is not convertable to ulong assume it is a manually entered quote
        else
        {
            //If manually entered quote has no origin then respond with unsuccessful message
            if (originUser is null)
            {
                await RespondAsync("Entry unsuccessful.", ephemeral: true);
                return;
            }
            addCommand = addMessageMapper.Map(messageEntry, title, Context, messageTypeString, originUser);
        }

        AddMessageResult addResult = await addMessageHandler.HandleAsync(addCommand);

        await RespondAsync(addResult.Message, ephemeral: true);
    }

    //Command to call an entry from the database based on ID
    [SlashCommand("call", "Call entry by ID from the database")]
    public async Task CallMessage([Summary("Query", "Message ID or Title")] string query)
    {
        CallMessageResult callResult = await callMessageHandler.HandleAsync(new CallMessageQuery(query, Context.Guild.Id));

        if (!callResult.Success || callResult.Message is null)
        {
            await RespondAsync(callResult.OutcomeMessage, ephemeral: true);
            return;
        }

        Message message = callResult.Message;

        if (message.DiscordMessageId is null && message.MessageType == MessageEntityType.Quote)
        {
            IGuildUser quoteOrigin = Context.Guild.GetUser(message.MessageOriginId);
            string responseContent = $"\"{message.Content}\" - {quoteOrigin.DisplayName}";
            await RespondAsync(responseContent);
        }
        else
        {

            string responseContent = message.Content ?? string.Empty;

            if (message.AttachmentUrls.Count > 0)
            {
                responseContent += Environment.NewLine;
                responseContent += string.Join(Environment.NewLine, message.AttachmentUrls);
            }
            try
            {
                await RespondAsync(responseContent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while responding to interaction");
            }
        }
    }

    [SlashCommand("delete", "Delete an entry from the database")]
    public async Task DeleteEntry(
        [Summary("Id", "Database ID")] int dbId)
    {
        DeleteMessageResult deleteResult = await deleteMessageHandler.HandleAsync(
            new DeleteMessageCommand(
                dbId,
                Context.Guild.Id,
                Context.User.Id,
                ((SocketGuildUser)Context.User).GuildPermissions.Has(GuildPermission.Administrator)));

        await RespondAsync(deleteResult.OutcomeMessage, ephemeral: true);
    }

    [SlashCommand("update", "Update entry in database")]
    public async Task UpdateEntry(
        [Summary("Id", "Message ID")] int dbId,
        [Summary("Title", "Updated title")] string? title = null,
        [Choice("Meme", "Meme")]
        [Choice("Quote", "Quote")]
        [Choice("Other", "Other")]
        [Summary("Type", "Updated type")] string? messageTypeString = null)
    {
        UpdateMessageResult updateResult = await updateMessageHandler.HandleAsync(
            new UpdateMessageCommand(
                dbId,
                Context.Guild.Id,
                Context.User.Id,
                ((SocketGuildUser)Context.User).GuildPermissions.Has(GuildPermission.Administrator),
                messageTypeString,
                title));

        await RespondAsync(updateResult.OutcomeMessage, ephemeral: true);
    }

    [SlashCommand("list", "List entries in database")]
    public async Task ListEntries([Summary("User", "list entries from a specific user")] SocketUser? user = null)
    {
        ListMessageResult listResult = await listMessageHandler.HandleAsync(new ListMessageQuery(Context.Guild.Id, user?.Id));

        if (listResult.Messages.Count == 0)
        {
            await RespondAsync("No messages found", ephemeral: true);
            return;
        }
        string idString = string.Join("\n", listResult.Messages.Select(m => m.Id));
        string titleString = string.Join("\n", listResult.Messages.Select(m => m.Title ?? "N/A"));
        string typeString = string.Join("\n", listResult.Messages.Select(m => m.MessageType.ToString()));
        string authorString = string.Join("\n", listResult.Messages.Select(m => m.Author));

        var embed = new EmbedBuilder();

        //Limited to 3 fields inline due to Discord css
        embed.AddField("Id", idString, true);
        embed.AddField("Title", titleString, true);
        embed.AddField("Type", typeString, true);
        embed.WithColor(Color.Gold);
        embed.WithTitle("Olli Bot Database");

        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}
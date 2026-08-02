using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Application.Commands.AddMessage;
using OlliBot.Application.Interfaces;
using OlliBot.Bot.Interfaces;
using OlliBot.Domain.Entities;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Modules.Commands;

[RequireContext(ContextType.Guild)]
[Group("db", "Commands to interact with the message database")]
public class DatabaseCommands(IMessageRepository messageService, ILogger<DatabaseCommands> logger, IMessageFactory messageFactory, AddMessageHandler addMessageHandler, AddMessageCommandMapper addMessageMapper) : InteractionModuleBase<SocketInteractionContext>
{

    //Command to add entries to database
    [SlashCommand("add", "Add a discord message to the database")]
    public async Task AddMessage([Summary("message", "Enter a message ID or quote content")] string messageEntry,
    [Summary("title", "Title (Optional)")] string? Title = null,
    [Summary("origin", "Quote origin (Optional if using Message ID for input)")] SocketGuildUser? User = null,
    [Choice("Meme", "Meme")]
    [Choice("Quote", "Quote")]
    [Choice("Other", "Other")]
    [Summary("Type", "Type (If no value set then will be implicitly determined)")] string? messageTypeString = null)
    {
        AddMessageCommand command;

        if (ulong.TryParse(messageEntry, out ulong result))
        {
            IMessage DiscordMessageObj = await Context.Interaction.Channel.GetMessageAsync(result);
            command = addMessageMapper.Map(DiscordMessageObj, Title, Context, messageTypeString);
        }
        //If input for message is not convertable to ulong assume it is a manually entered quote
        else
        {
            //If manually entered quote has no origin then respond with unsuccessful message
            if (User is null)
            {
                await Context.Interaction.RespondAsync("Entry unsuccessful, try again with a quote origin.", ephemeral: true);
                return;
            }
            command = addMessageMapper.Map(messageEntry, Title, Context, messageTypeString, User);
        }

        AddMessageResult commandResult = await addMessageHandler.HandleAsync(command);

        await RespondAsync(commandResult.Message, ephemeral: true);
    }

    //Command to call an entry from the database based on ID
    [SlashCommand("call", "Call entry by ID from the database")]
    public async Task CallMessage([Summary("Query", "Message ID or Title")] string query)
    {
        Message? queriedMessage;

        if (int.TryParse(query, out int intQuery))
        {
            queriedMessage = await messageService.GetByIdAsync(intQuery, Context.Guild.Id);
        }
        else
        {
            queriedMessage = await messageService.GetByTitleAsync(query, Context.Guild.Id);
        }

        if (queriedMessage is null)
        {
            await Context.Interaction.RespondAsync("No message found", ephemeral: true);

            return;
        }

        if (queriedMessage.DiscordMessageId is null && queriedMessage.MessageType == MessageEntityType.Quote)
        {
            IGuildUser quoteOrigin = Context.Guild.GetUser(queriedMessage.MessageOriginId);
            string responseContent = $"\"{queriedMessage.Content}\" - {quoteOrigin.DisplayName}";
            await Context.Interaction.RespondAsync(responseContent);
        }
        else
        {

            string responseContent = queriedMessage.Content ?? string.Empty;

            if (queriedMessage.AttachmentUrls.Count > 0)
            {
                responseContent += Environment.NewLine;
                foreach (string attachment in queriedMessage.AttachmentUrls)
                {
                    responseContent += attachment + Environment.NewLine;
                }
            }
            try
            {
                await Context.Interaction.RespondAsync(responseContent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while responding to interaction");
            }
        }
    }
    [SlashCommand("delete", "Delete an entry from the database")]
    public async Task DeleteEntry(
    [Summary("Id", "Database ID")] int DbID)
    {
        var user = (SocketGuildUser)Context.User;

        Message? queriedMessage = await messageService.GetByIdAsync(DbID, Context.Guild.Id);

        if (queriedMessage is null)
        {
            await Context.Interaction.RespondAsync("No Entry found", ephemeral: true);
            return;
        }

        if (queriedMessage.AuthorId != Context.User.Id && !user.GuildPermissions.Has(GuildPermission.Administrator))
        {
            await Context.Interaction.RespondAsync("You must be admin to delete database entries from other users", ephemeral: true);
            return;
        }

        await messageService.DeleteAsync(queriedMessage);
        await Context.Interaction.RespondAsync("Deleted entry", ephemeral: true);

    }
    [SlashCommand("update", "Update entry in database")]
    public async Task UpdateEntry(
    [Summary("Id", "Message ID")] int DbID,
    [Summary("Title", "Updated title")] string? Title = null,
    [Choice("Meme", "Meme")]
    [Choice("Quote", "Quote")]
    [Choice("Other", "Other")]
    [Summary("Type", "Updated type")] string? messageTypeString = null)
    {
        Message? queriedMessage = await messageService.GetByIdAsync(DbID, Context.Guild.Id);

        if (queriedMessage == null || Title == null && messageTypeString == null)
        {
            string x = "wat";
            await Context.Interaction.RespondAsync(x, ephemeral: true);
            return;
        }
        if (Title != null)
        {
            queriedMessage.Title = Title;
        }
        if (messageTypeString != null)
        {
            MessageEntityType messageType = messageTypeString switch
            {
                "Meme" => MessageEntityType.Meme,
                "Quote" => MessageEntityType.Quote,
                "Other" => MessageEntityType.Other,
                _ => MessageEntityType.Other
            };

            queriedMessage.MessageType = messageType;
        }

        await messageService.UpdateAsync(queriedMessage);
        await Context.Interaction.RespondAsync("Updated entry", ephemeral: true);
    }

    [SlashCommand("list", "List entries in database")]
    public async Task ListEntries([Summary("User", "list entries from a specific user")] SocketUser? user = null)
    {
        List<Message> messageList = await messageService.ListAsync(Context.Guild.Id, user?.Id);

        if (messageList.Count == 0)
        {
            await Context.Interaction.RespondAsync("No messages found", ephemeral: true);
            return;
        }
        string idString = string.Join("\n", messageList.Select(m => m.Id));
        string titleString = string.Join("\n", messageList.Select(m => m.Title ?? "N/A"));
        string typeString = string.Join("\n", messageList.Select(m => m.MessageType));
        string authorString = string.Join("\n", messageList.Select(m => m.Author));

        var embed = new EmbedBuilder();

        //Limited to 3 fields inline due to Discord css
        embed.AddField("Id", idString, true);
        embed.AddField("Title", titleString, true);
        embed.AddField("Type", typeString, true);
        embed.WithColor(Color.Gold);
        embed.WithTitle("Olli Bot Database");

        await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}
using Discord;
using Discord.Interactions;
using MediatR;
using OlliBot.Application.Messages.Commands;
using OlliBot.Bot.Mappers;
using ContextType = Discord.Interactions.ContextType;
using RequireContextAttribute = Discord.Interactions.RequireContextAttribute;

namespace OlliBot.Bot.Modules.MessageLibrary;

[RequireContext(ContextType.Guild)]
public class MessageLibraryContextCommands(
    ISender sender,
    IDiscordClient client,
    ILogger<MessageLibraryContextCommands> logger,
    AddMessageCommandMapper addMessageMapper)
    : InteractionModuleBase<SocketInteractionContext>
{
    [MessageCommand("Add Message")]
    public async Task AddMessageAsync(IMessage message)
    {
        if (message.Author.IsBot)
        {
            await RespondAsync("Cannot store messages sent by Bots", ephemeral: true);
            return;
        }

        await RespondWithModalAsync<AddMessageModal>($"add_message:{message.Id}");
    }

    [ModalInteraction("add_message:*")]
    public async Task HandleAddMessageModalAsync(string messageId, AddMessageModal modal)
    {
        AddMessageCommand addCommand;

        if (ulong.TryParse(messageId, out ulong result))
        {
            IMessage discordMessage = await Context.Interaction.Channel.GetMessageAsync(result);
            addCommand = addMessageMapper.Map(discordMessage, modal.MessageTitle, Context, modal.MessageType);

            AddMessageResult addResult = await sender.Send(addCommand);

            await RespondAsync(addResult.Message, ephemeral: true);
        }
        else
        {
            await RespondAsync("Failed to parse message id", ephemeral: true);
        }
    }
}
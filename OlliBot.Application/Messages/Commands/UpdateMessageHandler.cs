using OlliBot.Application.Interfaces;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.Messages.Commands;

public class UpdateMessageHandler(IMessageRepository messageRepository)
{
    public async Task<UpdateMessageResult> HandleAsync(UpdateMessageCommand command, CancellationToken cancellationToken = default)
    {
        Domain.Entities.Message? message = await messageRepository.GetByIdAsync(command.DbID, command.GuildId, cancellationToken);

        if (message is null)
        {
            return new UpdateMessageResult(false, "Can't find message to update.");
        }
        if (!command.InvokedByAdmin && message.AuthorId != command.InvokerUserId)
        {
            return new UpdateMessageResult(false, "You don't have permissions to update this message.");
        }

        message.Title = command.NewTitle ?? message.Title;

        if (Enum.TryParse<MessageEntityType>(command.NewType, out MessageEntityType messageType))
        {
            message.MessageType = messageType;
        }

        await messageRepository.UpdateAsync(message, cancellationToken);

        return new UpdateMessageResult(true, "Updated entry.");
    }
}

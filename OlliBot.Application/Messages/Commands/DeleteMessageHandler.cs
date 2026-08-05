using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.Commands;
public class DeleteMessageHandler(IMessageRepository messageRepository)
{
    public async Task<DeleteMessageResult> HandleAsync(DeleteMessageCommand command, CancellationToken cancellationToken = default)
    {
        Message? message = await messageRepository.GetByIdAsync(command.DbID, command.GuildId, cancellationToken);

        if (message == null)
        {
            return new DeleteMessageResult(false, "Message not found.");
        }

        if (message.AuthorId != command.InvokerUserId && !command.InvokedByAdmin)
        {
            return new DeleteMessageResult(false, "You must be admin to delete database entries from other users.");
        }

        await messageRepository.DeleteAsync(message, cancellationToken);

        return new DeleteMessageResult(true, "Message deleted successfully.");
    }
}

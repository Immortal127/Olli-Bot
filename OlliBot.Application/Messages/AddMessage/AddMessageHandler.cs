using MediatR;
using Microsoft.Extensions.Logging;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.AddMessage;

public class AddMessageHandler(
    ILogger<AddMessageHandler> logger,
    IMessageRepository messageRepository) : IRequestHandler<AddMessageCommand, AddMessageResult>
{
    public async Task<AddMessageResult> Handle(AddMessageCommand command, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            DiscordMessageId = command.DiscordMessageId,
            GuildId = command.GuildId,
            Title = command.Title,
            Content = command.Content,
            AttachmentUrls = command.AttachmentUrls,
            Author = command.Author,
            AuthorId = command.AuthorId,
            MessageOriginId = command.OriginUserId,
            DateTimeAdded = DateTime.UtcNow,
            MessageType = command.MessageType
        };

        try
        {
            await messageRepository.AddAsync(message, cancellationToken);
            return new AddMessageResult(true, "Entry added to the database");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add entry to database");
            return new AddMessageResult(false, "Failed to add entry.");
        }
    }
}

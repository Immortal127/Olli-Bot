using Microsoft.Extensions.Logging;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.Commands.AddMessage;

public class AddMessageHandler(ILogger<AddMessageHandler> logger, IMessageRepository messageRepository)
{
    public async Task<AddMessageResult> HandleAsync(AddMessageCommand command)
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
            await messageRepository.AddAsync(message);
            return new AddMessageResult(true, "Entry added to the database");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add entry to database");
            return new AddMessageResult(false, "Failed to add entry.");
        }
    }
}

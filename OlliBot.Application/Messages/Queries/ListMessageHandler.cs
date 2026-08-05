using OlliBot.Application.Interfaces;

namespace OlliBot.Application.Messages.Queries;
public class ListMessageHandler(IMessageRepository messageRepository)
{
    public async Task<ListMessageResult> HandleAsync(ListMessageQuery query, CancellationToken cancellationToken = default)
    {
        List<Domain.Entities.Message> messages = await messageRepository.ListAsync(query.GuildId, cancellationToken, query.UserId);
        return new ListMessageResult(messages, true);
    }
}
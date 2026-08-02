using OlliBot.Application.Interfaces;

namespace OlliBot.Application.Messages.Queries;
public class ListMessageHandler(IMessageRepository messageRepository)
{
    public async Task<ListMessageResult> HandleAsync(ListMessageQuery query, CancellationToken cancellationToken = default)
    {
        List<Domain.Entities.Message> messages = await messageRepository.ListAsync(query.GuildId, query.UserId, cancellationToken);
        return new ListMessageResult(messages, true);
    }
}
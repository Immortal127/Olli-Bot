using MediatR;
using OlliBot.Application.Interfaces;

namespace OlliBot.Application.Messages.Queries;
public class ListMessageHandler(IMessageRepository messageRepository) : IRequestHandler<ListMessageQuery, ListMessageResult>
{
    public async Task<ListMessageResult> Handle(ListMessageQuery query, CancellationToken cancellationToken = default)
    {
        List<Domain.Entities.Message> messages = await messageRepository.ListAsync(query.GuildId, cancellationToken, query.UserId);
        return new ListMessageResult(messages, true);
    }
}
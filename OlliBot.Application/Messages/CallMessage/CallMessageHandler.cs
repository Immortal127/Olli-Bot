using MediatR;
using Microsoft.Extensions.Logging;
using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.CallMessage;
public class CallMessageHandler(
    ILogger<CallMessageHandler> logger,
    IMessageRepository messageRepository) : IRequestHandler<CallMessageQuery, CallMessageResult>
{
    public async Task<CallMessageResult> Handle(CallMessageQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            Message? message =
                int.TryParse(query.Query, out int id)
                    ? await messageRepository.GetByIdAsync(id, query.GuildId, cancellationToken)
                    : await messageRepository.GetByTitleAsync(query.Query, query.GuildId, cancellationToken);

            if (message is null)
            {
                return new CallMessageResult(
                    false,
                    null,
                    "No message found");
            }

            return new CallMessageResult(
                true,
                message,
                null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while handling CallMessageQuery.");
            return new CallMessageResult(
                false,
                null,
                "Call message handler failed.");
        }
    }
}

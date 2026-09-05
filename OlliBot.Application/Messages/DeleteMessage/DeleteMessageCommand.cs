using MediatR;

namespace OlliBot.Application.Messages.DeleteMessage;
public record DeleteMessageCommand(
    int DbID,
    ulong GuildId,
    ulong InvokerUserId,
    bool InvokedByAdmin) : IRequest<DeleteMessageResult>;
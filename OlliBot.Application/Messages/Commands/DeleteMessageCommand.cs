using MediatR;

namespace OlliBot.Application.Messages.Commands;
public record DeleteMessageCommand(
    int DbID,
    ulong GuildId,
    ulong InvokerUserId,
    bool InvokedByAdmin) : IRequest<DeleteMessageResult>;
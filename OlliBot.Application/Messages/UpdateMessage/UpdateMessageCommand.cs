using MediatR;

namespace OlliBot.Application.Messages.UpdateMessage;
public record UpdateMessageCommand(
    int DbID,
    ulong GuildId,
    ulong InvokerUserId,
    bool InvokedByAdmin,
    string? NewType,
    string? NewTitle) : IRequest<UpdateMessageResult>;
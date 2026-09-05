using MediatR;

namespace OlliBot.Application.Messages.ListMessages;

public record ListMessageQuery(
    ulong GuildId,
    ulong? UserId) : IRequest<ListMessageResult>;
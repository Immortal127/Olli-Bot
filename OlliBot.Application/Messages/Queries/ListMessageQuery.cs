using MediatR;

namespace OlliBot.Application.Messages.Queries;

public record ListMessageQuery(
    ulong GuildId,
    ulong? UserId) : IRequest<ListMessageResult>;
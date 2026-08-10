using MediatR;

namespace OlliBot.Application.Messages.Queries;

public sealed record CallMessageQuery(
    string Query,
    ulong GuildId) : IRequest<CallMessageResult>;
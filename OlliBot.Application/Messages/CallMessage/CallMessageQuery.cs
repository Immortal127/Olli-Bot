using MediatR;

namespace OlliBot.Application.Messages.CallMessage;

public sealed record CallMessageQuery(
    string Query,
    ulong GuildId) : IRequest<CallMessageResult>;
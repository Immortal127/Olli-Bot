using MediatR;

namespace OlliBot.Application.EmoteRanking.Commands;

public record ClearEmoteRankingCommand(ulong GuildId, bool IsInvokedByAdmin) : IRequest<ClearEmoteRankingResult>;
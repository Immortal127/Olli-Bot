using MediatR;

namespace OlliBot.Application.EmoteRanking.ClearEmoteRanking;

public record ClearEmoteRankingCommand(ulong GuildId, bool IsInvokedByAdmin) : IRequest<ClearEmoteRankingResult>;
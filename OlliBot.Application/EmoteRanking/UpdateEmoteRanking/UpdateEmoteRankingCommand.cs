using MediatR;

namespace OlliBot.Application.EmoteRanking.UpdateEmoteRanking;
public record UpdateEmoteRankingCommand(ulong GuildId) : IRequest<UpdateEmoteRankingResult>;
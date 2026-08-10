using MediatR;

namespace OlliBot.Application.EmoteRanking.Commands;
public record UpdateEmoteRankingCommand(ulong GuildId) : IRequest<UpdateEmoteRankingResult>;
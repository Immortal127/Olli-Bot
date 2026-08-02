using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.Queries;
public record ListMessageResult(List<Message> Messages, bool Success);

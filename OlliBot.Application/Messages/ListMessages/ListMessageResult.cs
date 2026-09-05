using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.ListMessages;
public record ListMessageResult(
    List<Message> Messages,
    bool Success);

using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.CallMessage;
public sealed record CallMessageResult(
    bool Success,
    Message? Message,
    string? OutcomeMessage);
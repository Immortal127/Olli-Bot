using OlliBot.Domain.Entities;

namespace OlliBot.Application.Messages.Queries;
public sealed record CallMessageResult(
    bool Success,
    Message? Message,
    string? OutcomeMessage);
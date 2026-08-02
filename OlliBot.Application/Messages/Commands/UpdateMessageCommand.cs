namespace OlliBot.Application.Messages.Commands;
public record UpdateMessageCommand(
    int DbID,
    ulong GuildId,
    ulong InvokerUserId,
    bool InvokedByAdmin,
    string? NewType,
    string? NewTitle);
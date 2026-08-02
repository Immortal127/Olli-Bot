using OlliBot.Domain.Enums;

namespace OlliBot.Application.Commands.AddMessage;

public sealed record AddMessageCommand(
    ulong? DiscordMessageId,
    ulong GuildId,
    string Content,
    string? Title,
    string Author,
    ulong AuthorId,
    ulong OriginUserId,
    MessageEntityType MessageType,
    List<string> AttachmentUrls);
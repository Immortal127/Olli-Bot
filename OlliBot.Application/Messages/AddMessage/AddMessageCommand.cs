using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.Messages.AddMessage;

public sealed record AddMessageCommand(
    ulong? DiscordMessageId,
    ulong GuildId,
    string Content,
    string? Title,
    string Author,
    ulong AuthorId,
    ulong OriginUserId,
    MessageEntityType MessageType,
    List<string> AttachmentUrls) : IRequest<AddMessageResult>;
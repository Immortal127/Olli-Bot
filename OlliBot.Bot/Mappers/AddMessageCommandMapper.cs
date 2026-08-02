using Discord;
using OlliBot.Application.Messages.Commands;
using OlliBot.Domain.Enums;
using System.Text.RegularExpressions;

namespace OlliBot.Bot.Mappers;

public class AddMessageCommandMapper
{
    private static readonly string[] MemeExtensions =
        [
            ".png",
            ".jpeg",
            ".jpg",
            ".gif",
            ".mp4"
        ];

    private static readonly Regex UrlRegex = new(@"https?://[^\s/$.?#].[^\s]*", RegexOptions.Compiled);

    public AddMessageCommand Map(
        IMessage message,
        string? title,
        IInteractionContext context,
        string? messageTypeString)
    {
        var attachmentUrls = message.Attachments
            .Select(a => a.Url)
            .ToList();

        string content = message.Content;


        MatchCollection matches = UrlRegex.Matches(content);

        if (matches.Count > 0)
        {
            attachmentUrls.AddRange(matches.Select(m => m.Value));
            content = UrlRegex.Replace(content, string.Empty).Trim();
        }

        MessageEntityType messageType = messageTypeString switch
        {
            "Meme" => MessageEntityType.Meme,
            "Quote" => MessageEntityType.Quote,
            "Other" => MessageEntityType.Other,
            _ => DetermineMessageType(attachmentUrls, content)
        };

        return new AddMessageCommand(
            DiscordMessageId: message.Id,
            GuildId: context.Guild.Id,
            Title: title,
            Content: content,
            AttachmentUrls: attachmentUrls,
            Author: context.User.Username,
            AuthorId: context.User.Id,
            OriginUserId: message.Author.Id,
            MessageType: messageType);
    }

    public AddMessageCommand Map(
    string messageContent,
    string? title,
    IInteractionContext context,
    string? messageTypeString,
    IUser originUser)
    {
        var attachmentUrls = new List<string>();

        string content = messageContent;

        MatchCollection matches = UrlRegex.Matches(content);

        if (matches.Count > 0)
        {
            attachmentUrls.AddRange(matches.Select(m => m.Value));
            content = UrlRegex.Replace(content, string.Empty).Trim();
        }

        MessageEntityType messageType = messageTypeString switch
        {
            "Meme" => MessageEntityType.Meme,
            "Quote" => MessageEntityType.Quote,
            "Other" => MessageEntityType.Other,
            _ => DetermineMessageType(attachmentUrls, content)
        };

        return new AddMessageCommand(
            DiscordMessageId: null, // or 0, depending on your command
            GuildId: context.Guild.Id,
            Title: title,
            Content: content,
            AttachmentUrls: attachmentUrls,
            Author: context.User.Username,
            AuthorId: context.User.Id,
            OriginUserId: originUser.Id,
            MessageType: messageType);
    }

    private static bool IsMemeAttachment(string url)
    {
        string extension = Path.GetExtension(url);
        return MemeExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    private static MessageEntityType DetermineMessageType(IReadOnlyCollection<string> attachmentUrls, string content)
    {
        if (attachmentUrls.Any(IsMemeAttachment))
        {
            return MessageEntityType.Meme;
        }
        if (!string.IsNullOrWhiteSpace(content) && attachmentUrls.Count == 0)
        {
            return MessageEntityType.Quote;
        }

        return MessageEntityType.Other;
    }
}
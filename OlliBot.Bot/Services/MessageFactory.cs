using Discord;
using OlliBot.Bot.Interfaces;
using OlliBot.Bot.Utilities;
using OlliBot.Domain.Entities;
using System.Text.RegularExpressions;

namespace OlliBot.Bot.Services;

public class MessageFactory : IMessageFactory
{
    public Message CreateMessageFromInput(IMessage message, string? Title, IInteractionContext ctx, string? messageType)
    {
        var attList = new List<string>();

        string entryContent = message.Content;

        //Attachment is a file upload attached to a message
        if (message.Attachments.Count > 0)
        {
            foreach (IAttachment attachment in message.Attachments)
            {
                attList.Add(attachment.Url);
            }
        }

        var entry = new Message
        {
            DiscordMessageId = message.Id,
            GuildId = ctx.Guild.Id,
            Title = Title,
            Content = entryContent,
            AttachmentUrls = attList,
            Author = ctx.User.Username,
            AuthorId = ctx.User.Id,
            MessageOriginId = message.Author.Id,
            DateTimeAdded = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(entry.Content) && Helpers.HasURL(entry.Content))
        {
            var regex = new Regex(@"https?://[^\s/$.?#].[^\s]*");
            var matches = regex.Matches(entry.Content).Select(m => m.Value).ToList();

            entry.AttachmentUrls.AddRange(matches);

            entry.Content = regex.Replace(entry.Content, string.Empty);
        }

        entry.MessageType = messageType ?? EvaluateMessageType(entry);

        return entry;
    }
    public Message CreateMessageFromInput(string messageContent, string? Title, IInteractionContext ctx, string? messageType, IUser User)
    {
        var entry = new Message
        {
            GuildId = ctx.Guild.Id,
            Title = Title,
            Content = messageContent,
            Author = ctx.User.Username,
            AuthorId = ctx.User.Id,
            MessageOriginId = User.Id,
            DateTimeAdded = DateTime.UtcNow,
        };

        if (!string.IsNullOrEmpty(entry.Content) && Helpers.HasURL(entry.Content))
        {
            var regex = new Regex(@"https?://[^\s/$.?#].[^\s]*");

            var matches = regex.Matches(entry.Content).Select(m => m.Value).ToList();

            List<string> currentAttachments = entry.AttachmentUrls;
            currentAttachments.AddRange(matches);
            entry.AttachmentUrls = currentAttachments;

            entry.Content = regex.Replace(entry.Content, string.Empty).Trim();
        }

        entry.MessageType = messageType ?? EvaluateMessageType(entry);

        return entry;
    }
    public string EvaluateMessageType(Message message)
    {
        var memeExtensions = new List<string> { ".png", ".jpeg", ".jpg", ".gif", ".mp4" };
        if (message.AttachmentUrls.Count > 0 && (message.AttachmentUrls.Any(url => memeExtensions.Any(ex => url.Contains(ex)))))
        {
            return "Meme";
        }
        else if (!string.IsNullOrEmpty(message.Content) && message.AttachmentUrls.Count == 0)
        {
            return "Quote";
        }
        else
        {
            return "Other";
        }
    }
}

using Discord;
using OlliBot.Domain.Entities;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Interfaces;

public interface IMessageFactory
{
    Message CreateMessageFromInput(IMessage message, string? Title, IInteractionContext ctx, MessageEntityType? messageType);
    Message CreateMessageFromInput(string messageContent, string? Title, IInteractionContext ctx, MessageEntityType? messageType, IUser User);
    MessageEntityType EvaluateMessageType(Message message);
}
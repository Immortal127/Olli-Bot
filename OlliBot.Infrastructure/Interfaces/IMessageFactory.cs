using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Interfaces;

public interface IMessageFactory
{
    Message CreateMessageFromInput(IMessage message, string? Title, IInteractionContext ctx, string? messageType);
    Message CreateMessageFromInput(string messageContent, string? Title, IInteractionContext ctx, string? messageType, IUser User);
    string EvaluateMessageType(Message message);
}
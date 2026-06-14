
using OlliBot.Infrastructure.Entities;

namespace OlliBot.Infrastructure.Interfaces;

public interface IMessageService
{
    Task AddMessageAsync(Message message);
    Task DeleteMessageAsync(Message message);
    Task<Message?> GetMessageByIdAsync(int id, ulong guildId);
    Task<Message?> GetMessageByTitleAsync(string Title, ulong guildId);
    Task<List<Message>> ListMessagesAsync(ulong guildId, ulong? userId = null);
    Task UpdateMessageAsync(Message message);
}
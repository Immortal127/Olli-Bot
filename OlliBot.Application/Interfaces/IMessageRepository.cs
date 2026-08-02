using OlliBot.Domain.Entities;

namespace OlliBot.Application.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct = default);

    Task DeleteAsync(Message message, CancellationToken ct = default);

    Task<Message?> GetByIdAsync(int id, ulong guildId, CancellationToken ct = default);

    Task<Message?> GetByTitleAsync(string title, ulong guildId, CancellationToken ct = default);

    Task<List<Message>> ListAsync(ulong guildId, ulong? userId = null, CancellationToken ct = default);

    Task UpdateAsync(Message message, CancellationToken ct = default);
}
using OlliBot.Domain.Entities;

namespace OlliBot.Application.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct);

    Task DeleteAsync(Message message, CancellationToken ct);

    Task<Message?> GetByIdAsync(int id, ulong guildId, CancellationToken ct);

    Task<Message?> GetByTitleAsync(string title, ulong guildId, CancellationToken ct);

    Task<List<Message>> ListAsync(ulong guildId, CancellationToken ct, ulong? userId = null);

    Task UpdateAsync(Message message, CancellationToken ct);
}
using Microsoft.EntityFrameworkCore;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Entities;
using OlliBot.Infrastructure.Data;

namespace OlliBot.Infrastructure.Repositories;

public class MessageRepository(OlliBotDbContext db) : IMessageRepository
{
    public async Task AddAsync(Message message, CancellationToken ct = default)
    {
        await db.Messages.AddAsync(message, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Message message, CancellationToken ct = default)
    {
        db.Messages.Remove(message);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Message message, CancellationToken ct = default)
    {
        db.Messages.Update(message);
        await db.SaveChangesAsync(ct);
    }

    public async Task<Message?> GetByIdAsync(int id, ulong guildId, CancellationToken ct = default)
    {
        return await db.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.GuildId == guildId, ct);
    }

    public async Task<Message?> GetByTitleAsync(string title, ulong guildId, CancellationToken ct = default)
    {
        return await db.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Title != null && m.GuildId == guildId && m.Title.ToLower().Contains(title), ct);
    }

    public async Task<List<Message>> ListAsync(ulong guildId, ulong? userId = null, CancellationToken ct = default)
    {
        IQueryable<Message> messageQuery = db.Messages
            .AsNoTracking()
            .Where(m => m.GuildId == guildId);

        if (userId != null)
        {
            messageQuery = messageQuery.Where(m => m.AuthorId == userId);
        }

        List<Message> messageList = await messageQuery.ToListAsync(ct);

        return messageList;
    }
}

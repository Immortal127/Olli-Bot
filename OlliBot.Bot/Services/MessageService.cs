using Microsoft.EntityFrameworkCore;
using OlliBot.Bot.Interfaces;
using OlliBot.Domain.Entities;
using OlliBot.Infrastructure.Data;

namespace OlliBot.Bot.Services;

public class MessageService(OlliBotDbContext db) : IMessageService
{
    public async Task AddMessageAsync(Message message)
    {

        await db.Messages.AddAsync(message);
        await db.SaveChangesAsync();
    }
    public async Task DeleteMessageAsync(Message message)
    {

        db.Messages.Remove(message);
        await db.SaveChangesAsync();
    }
    public async Task UpdateMessageAsync(Message message)
    {

        db.Messages.Update(message);
        await db.SaveChangesAsync();
    }
    public async Task<Message?> GetMessageByIdAsync(int id, ulong guildId)
    {

        Message? message = await db.Messages.FirstOrDefaultAsync(m => m.Id == id && m.GuildId == guildId);
        return message;
    }
    public async Task<Message?> GetMessageByTitleAsync(string Title, ulong guildId)
    {

        Message? message = await db.Messages.FirstOrDefaultAsync(m => m.Title != null && m.GuildId == guildId && m.Title.ToLower().Contains(Title));
        return message;

    }
    public async Task<List<Message>> ListMessagesAsync(ulong guildId, ulong? userId = null)
    {

        IQueryable<Message> messageQuery = db.Messages.Where(m => m.GuildId == guildId);

        if (userId != null)
        {
            messageQuery = messageQuery.Where(m => m.AuthorId == userId);
        }

        List<Message> messageList = await messageQuery.ToListAsync();

        return messageList;
    }
}

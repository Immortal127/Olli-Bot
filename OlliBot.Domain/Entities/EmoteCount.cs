namespace OlliBot.Domain.Entities;
public class EmoteCount
{
    public ulong GuildId { get; set; }
    public ulong EmoteId { get; set; }
    public int Count { get; set; } = 0;
    public DateTime DateTimeUpdated { get; set; }
}
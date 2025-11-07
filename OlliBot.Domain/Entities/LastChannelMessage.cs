namespace OlliBot.Domain.Entities;
public class LastChannelMessage
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
}
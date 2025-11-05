using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace OlliBot.Data;

public class MessageDB : DbContext
{
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ServersData.db");
        optionsBuilder.EnableSensitiveDataLogging(true);
    }
}

public class Message
{
    /// <summary>
    /// The Id of the Message in the database
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The Id assigned to the discord message within discord
    /// </summary>
    public ulong? DiscordMessageId { get; set; }
    /// <summary>
    /// The Id of the discord server the Message was entered into the db from
    /// </summary>
    public ulong GuildId { get; set; }
    /// <summary>
    /// The title of the Message
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Text content of the Message
    /// </summary>
    public string? Content { get; set; }
    /// <summary>
    /// The attachments (urls, images, etc) of the Message serialized as a string
    /// </summary>
    public string? Attachments { get; set; }
    /// <summary>
    /// The attachments of the Message deserialized as a json object. This property is not mapped to a column in the database.
    /// </summary>
    [NotMapped]
    public List<string> AttachmentUrls
    {
        get => Attachments == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(Attachments) ?? new List<string>();
        set => Attachments = JsonSerializer.Serialize(value);
    }
    /// <summary>
    /// User that added the Message into the database
    /// </summary>
    public required string Author { get; set; }
    /// <summary>
    /// Discord user Id of the Author
    /// </summary>
    public ulong AuthorId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ulong MessageOriginId { get; set; }

    public string MessageType { get; set; } = "Other";
    /// <summary>
    /// When was the Message added to the database.
    /// </summary>
    public required DateTime DateTimeAdded { get; set; }
}
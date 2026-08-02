using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlliBot.Domain.Entities;
using System.Text.Json;

namespace OlliBot.Infrastructure.Data.Configurations;
public sealed class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.Property(x => x.GuildId);
        b.Property(x => x.AuthorId);
        b.Property(x => x.MessageOriginId);
        b.Property(x => x.DiscordMessageId);

        // store attachments as JSON text
        b.Property(m => m.AttachmentUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        b.Property(x => x.MessageType);
        b.Property(x => x.Author).IsRequired();
        b.Property(x => x.DateTimeAdded).IsRequired();
        b.HasIndex(x => new { x.GuildId, x.MessageOriginId });
    }
}
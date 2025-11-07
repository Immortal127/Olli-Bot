using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Data.Configurations;
public sealed class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.Property(x => x.GuildId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.AuthorId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.MessageOriginId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.DiscordMessageId);//.HasConversion(Converters.NullableULongToLong);

        // store attachments as JSON text
        b.Property(x => x.AttachmentUrls);//.HasConversion(Converters.StringListToJson);

        b.Property(x => x.Author).IsRequired();
        b.Property(x => x.DateTimeAdded).IsRequired();
        b.HasIndex(x => new { x.GuildId, x.MessageOriginId });
    }
}
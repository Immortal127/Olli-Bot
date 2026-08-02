using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Data.Configurations;

public sealed class LastChannelMessageConfig : IEntityTypeConfiguration<LastChannelMessage>
{
    public void Configure(EntityTypeBuilder<LastChannelMessage> b)
    {
        b.HasKey(x => new { x.GuildId, x.ChannelId });
        b.Property(x => x.GuildId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.ChannelId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.MessageId);//.HasConversion(Converters.ULongToLong);
    }
}
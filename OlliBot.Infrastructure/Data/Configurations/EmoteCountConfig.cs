using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Data.Configurations;
public sealed class EmoteCountConfig : IEntityTypeConfiguration<EmoteCount>
{
    public void Configure(EntityTypeBuilder<EmoteCount> b)
    {
        b.HasKey(x => new { x.GuildId, x.EmoteId });
        b.Property(x => x.GuildId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.EmoteId);//.HasConversion(Converters.ULongToLong);
    }
}
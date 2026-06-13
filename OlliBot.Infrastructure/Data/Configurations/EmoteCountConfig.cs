using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OlliBot.Infrastructure.Data.Configurations;
public sealed class EmoteCountConfig : IEntityTypeConfiguration<EmoteCount>
{
    public void Configure(EntityTypeBuilder<EmoteCount> b)
    {
        b.HasKey(x => new { x.GuildId, x.EmoteId });
        // WTF does .Property do?
        b.Property(x => x.GuildId);//.HasConversion(Converters.ULongToLong);
        b.Property(x => x.EmoteId);//.HasConversion(Converters.ULongToLong);
    }
}

/*
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmoteCount>()
            .HasKey(e => new { e.GuildId, e.EmoteId });

        modelBuilder.Entity<LastChannelMessage>()
            .HasKey(m => new { m.GuildId, m.ChannelId });
    }
*/
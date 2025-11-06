using Microsoft.EntityFrameworkCore;
using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Data;
public class OlliBotDbContext : DbContext
{
    public OlliBotDbContext(DbContextOptions<OlliBotDbContext> options) : base(options) { }

    public DbSet<Message> Messages => Set<Message>();
    public DbSet<EmoteCount> EmoteCounts => Set<EmoteCount>();
    public DbSet<LastChannelMessage> LastChannelMessages => Set<LastChannelMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OlliBotDbContext).Assembly);
}
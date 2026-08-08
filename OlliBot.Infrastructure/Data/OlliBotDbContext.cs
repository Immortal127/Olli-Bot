using Microsoft.EntityFrameworkCore;
using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Data;
public class OlliBotDbContext : DbContext
{
    public OlliBotDbContext(DbContextOptions<OlliBotDbContext> options) : base(options) { }

    public DbSet<Message> Messages => Set<Message>();
    public DbSet<EmoteCount> EmoteCounts => Set<EmoteCount>();
    public DbSet<LastChannelMessage> LastChannelMessages => Set<LastChannelMessage>();

    public DbSet<Domain.Entities.HumbleBundle> HumbleBundles => Set<Domain.Entities.HumbleBundle>();

    public DbSet<HumbleBundleSubscriber> HumbleBundleSubscribers => Set<HumbleBundleSubscriber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OlliBotDbContext).Assembly);
}










/*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ServersData.db");
        optionsBuilder.EnableSensitiveDataLogging(true);
    }
*/
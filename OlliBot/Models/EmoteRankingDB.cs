using Microsoft.EntityFrameworkCore;

namespace OlliBot.Data
{
    public class EmoteRankingDB : DbContext
    {
        public DbSet<Emote> EmoteCounts { get; set; }
        public DbSet<LastChannelMessage> LastChannelMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ServersData.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Emote>()
                .HasKey(e => new { e.GuildId, e.EmoteId });

            modelBuilder.Entity<LastChannelMessage>()
                .HasKey(m => new { m.GuildId, m.ChannelId });
        }
    }

    public class Emote
    {
        public ulong GuildId { get; set; }
        public  ulong EmoteId {get; set; }
        public int Count { get; set; } = 0;
        public DateTime DateTimeUpdated { get; set; }
    }
    public class LastChannelMessage
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
    }
}
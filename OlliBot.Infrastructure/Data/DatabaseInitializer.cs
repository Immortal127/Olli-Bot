using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OlliBot.Infrastructure.Data;

public sealed class DatabaseInitializer(
    OlliBotDbContext db,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (!await db.Database.CanConnectAsync(ct))
        {
            logger.LogInformation("Database not found. Creating database and applying migrations...");
        }
        else
        {
            logger.LogInformation("Checking for pending database migrations...");
        }

        await db.Database.MigrateAsync(ct);

        logger.LogInformation("Database is ready.");
    }
}
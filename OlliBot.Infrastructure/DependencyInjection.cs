using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Application.EmoteRanking;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.Messages;
using OlliBot.Infrastructure.Data;
using OlliBot.Infrastructure.HumbleBundle;
using OlliBot.Infrastructure.Repositories;

namespace OlliBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        string connectionString =
            cfg.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<OlliBotDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IEmoteCountRepository, EmoteCountRepository>();
        services.AddScoped<IHumbleBundleRepository, HumbleBundleRepository>();

        services.AddScoped<DatabaseInitializer>();

        services.AddTransient<IHumbleBundleScanner, HumbleBundleScanner>();

        return services;
    }
}

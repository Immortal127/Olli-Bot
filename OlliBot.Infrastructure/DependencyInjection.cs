using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Application.Interfaces;
using OlliBot.Infrastructure.Data;
using OlliBot.Infrastructure.Repositories;

namespace OlliBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<OlliBotDbContext>(o =>
            o.UseSqlite(cfg.GetConnectionString("DefaultConnection") ?? "Data Source=ServersData.db"));

        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddTransient<DatabaseInitializer>();

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Infrastructure.Data;
using OlliBot.Infrastructure.Interfaces;
using OlliBot.Infrastructure.Services;

namespace OlliBot.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<OlliBotDbContext>(o =>
            o.UseSqlite(cfg.GetConnectionString("DefaultConnection") ?? "Data Source=ServersData.db"));

        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}

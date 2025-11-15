using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Infrastructure.Data;

namespace OlliBot.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<OlliBotDbContext>(o =>
            o.UseSqlite(cfg.GetConnectionString("DefaultConnection") ?? "Data Source=ServersData.db"));


        //services.AddTransient<IMessageFactory, MessageFactory>();
        //services.AddScoped<IMessageService, MessageService>();
        //services.AddScoped<IEmoteRankingService, EmoteRankingService>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using OlliBot.Application.Interfaces;

// Build action set to none until I figure out if this is a good idea

namespace OlliBot.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration cfg)
    {
        // services

        builder.Services.AddTransient<IMessageService, MessageService>();
        builder.Services.AddTransient<IMessageFactory, MessageFactory>();
        builder.Services.AddTransient<IEmoteRankingService, EmoteRankingService>();

        return services;
    }
}

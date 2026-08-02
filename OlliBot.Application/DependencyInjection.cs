using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Application.Commands.AddMessage;

namespace OlliBot.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration cfg)
    {
        // services

        services.AddTransient<AddMessageHandler>();


        //builder.Services.AddTransient<IMessageService, MessageService>();
        //builder.Services.AddTransient<IMessageFactory, MessageFactory>();
        //builder.Services.AddTransient<IEmoteRankingService, EmoteRankingService>();

        return services;
    }
}
